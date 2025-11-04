#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
从CSV文件中读取所有项目，生成格式化的预设数据
"""

import os
import json
import csv
import sys
from pathlib import Path


def find_json_file(models_dir, face_id):
    """
    在models目录中查找与捏脸ID匹配的JSON文件
    支持精确匹配和模糊匹配（去除特殊字符）
    """
    if not os.path.exists(models_dir):
        return None
    
    # 首先尝试精确匹配
    exact_path = os.path.join(models_dir, f"{face_id}.json")
    if os.path.exists(exact_path):
        return exact_path
    
    # 如果精确匹配失败，尝试模糊匹配
    # 移除空格、特殊字符等
    def normalize_name(name):
        """规范化名称，移除空格和常见特殊字符"""
        normalized = name.replace(" ", "").replace("?", "").replace("!", "").replace("。", "").replace("，", "")
        return normalized
    
    normalized_id = normalize_name(face_id)
    
    # 候选文件列表（使用字典避免重复）
    candidates = {}
    
    for filename in os.listdir(models_dir):
        if not filename.endswith('.json'):
            continue
            
        name_without_ext = filename[:-5]  # 移除 .json
        
        # 精确匹配
        if name_without_ext == face_id:
            return os.path.join(models_dir, filename)
        
        # 规范化匹配
        normalized_filename = normalize_name(name_without_ext)
        if normalized_filename == normalized_id:
            candidates[name_without_ext] = filename
        
        # 包含匹配（face_id包含在文件名中，或文件名包含在face_id中）
        if face_id in name_without_ext or name_without_ext in face_id:
            candidates[name_without_ext] = filename
    
    # 如果有候选文件，返回第一个（最匹配的）
    if candidates:
        # 优先返回包含匹配且长度最接近的
        sorted_candidates = sorted(candidates.items(), key=lambda x: abs(len(x[0]) - len(face_id)))
        return os.path.join(models_dir, sorted_candidates[0][1])
    
    return None


def escape_json_string(json_str):
    """
    将JSON字符串转义，使其可以在C#代码中使用
    需要转义：双引号 " -> \"
    注意：先转义反斜杠，再转义双引号，避免重复转义
    """
    # 先转义反斜杠（避免后面转义双引号时影响）
    escaped = json_str.replace('\\', '\\\\')
    # 再转义双引号
    escaped = escaped.replace('"', '\\"')
    return escaped


def read_csv_and_generate(csv_path, models_dir, output_path):
    """
    读取CSV文件，生成格式化的预设数据
    """
    print(f"正在读取CSV文件: {csv_path}")
    
    # 打开CSV文件
    try:
        with open(csv_path, 'r', encoding='utf-8') as f:
            csv_reader = csv.reader(f)
            rows = list(csv_reader)
    except Exception as e:
        print(f"错误: 无法打开CSV文件: {e}")
        return False
    
    if len(rows) < 2:
        print("错误: CSV文件格式不正确（至少需要表头行和一行数据）")
        return False
    
    # 第一行是表头
    header = rows[0]
    
    # 查找列索引
    face_id_col = None
    
    for col_idx, col_name in enumerate(header):
        col_name_clean = col_name.strip()
        if "捏脸ID" in col_name_clean or ("ID" in col_name_clean and "捏脸" in col_name_clean):
            face_id_col = col_idx
            break
    
    if face_id_col is None:
        print("错误: 无法找到必要的列（需要包含'捏脸ID'列）")
        print(f"找到的列: {header}")
        return False
    
    print(f"找到表头行: {header}")
    print(f"捏脸ID列索引: {face_id_col}")
    
    # 收集所有项目
    items = []
    
    # 从第二行开始读取数据
    for row_idx, row in enumerate(rows[1:], start=2):
        # 跳过空行
        if not row or all(not cell.strip() for cell in row):
            continue
        
        # 获取捏脸ID
        if face_id_col < len(row):
            face_id = row[face_id_col].strip()
        else:
            face_id = ""
        
        # 如果捏脸ID不为空，则添加到处理列表
        if face_id:
            items.append((row_idx, face_id))
            print(f"第{row_idx}行: 找到项目 - {face_id}")
    
    print(f"\n共找到 {len(items)} 个项目")
    
    # 生成输出内容
    output_lines = []
    
    for row_idx, face_id in items:
        # 查找对应的JSON文件
        json_path = find_json_file(models_dir, face_id)
        
        if not json_path or not os.path.exists(json_path):
            print(f"警告: 第{row_idx}行 '{face_id}' 未找到对应的JSON文件，跳过")
            continue
        
        # 读取JSON文件
        try:
            with open(json_path, 'r', encoding='utf-8') as f:
                json_content = f.read().strip()
            
            # 验证JSON格式
            try:
                json.loads(json_content)
            except json.JSONDecodeError as e:
                print(f"警告: 第{row_idx}行 '{face_id}' 的JSON文件格式错误: {e}，跳过")
                continue
            
            # 转义JSON字符串
            escaped_json = escape_json_string(json_content)
            
            # 生成输出行
            # 格式：{ "名称", "JSON字符串" },
            output_line = '            {{ "{}", "{}" }},'.format(face_id, escaped_json)
            output_lines.append(output_line)
            
            print(f"✓ 处理完成: {face_id}")
            
        except Exception as e:
            print(f"错误: 处理 '{face_id}' 时出错: {e}")
            continue
    
    # 写入输出文件
    try:
        with open(output_path, 'w', encoding='utf-8') as f:
            for line in output_lines:
                f.write(line + '\n')
        
        print(f"\n成功生成输出文件: {output_path}")
        print(f"共生成 {len(output_lines)} 行数据")
        return True
        
    except Exception as e:
        print(f"错误: 写入输出文件失败: {e}")
        return False


def main():
    # 获取脚本所在目录
    script_dir = Path(__file__).parent.absolute()
    
    # 文件路径
    csv_path = script_dir / "来源表.csv"
    models_dir = script_dir / "models"
    output_path = script_dir / "preset_data_output.txt"
    
    # 检查文件是否存在
    if not csv_path.exists():
        print(f"错误: CSV文件不存在: {csv_path}")
        return
    
    if not models_dir.exists():
        print(f"错误: models目录不存在: {models_dir}")
        return
    
    print("=" * 60)
    print("预设数据生成工具")
    print("=" * 60)
    print(f"CSV文件: {csv_path}")
    print(f"Models目录: {models_dir}")
    print(f"输出文件: {output_path}")
    print("=" * 60)
    print()
    
    # 执行生成
    success = read_csv_and_generate(
        str(csv_path),
        str(models_dir),
        str(output_path)
    )
    
    if success:
        print("\n" + "=" * 60)
        print("生成完成！")
        print("=" * 60)
    else:
        print("\n" + "=" * 60)
        print("生成失败！")
        print("=" * 60)


if __name__ == "__main__":
    main()

