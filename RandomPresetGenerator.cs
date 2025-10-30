using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MoreAppearancePreset
{
    /// <summary>
    /// 随机预设生成器
    /// </summary>
    public static class RandomPresetGenerator
    {
        private static System.Random _random = new System.Random();

        /// <summary>
        /// 生成随机preset数据
        /// </summary>
        public static string GenerateRandomPreset()
        {
            try
            {
                // 尝试从多个可能的位置读取预设范围配置文件
                string[] possiblePaths = new[]
                {
                    Path.Combine(Application.dataPath, "..", "预设范围.json"),
                    Path.Combine(Application.dataPath, "预设范围.json"),
                    Path.Combine(Directory.GetCurrentDirectory(), "预设范围.json"),
                    "预设范围.json"
                };

                string? configPath = null;
                foreach (string path in possiblePaths)
                {
                    try
                    {
                        if (File.Exists(path))
                        {
                            configPath = path;
                            Debug.Log($"[RandomPresetGenerator] 找到配置文件: {path}");
                            break;
                        }
                    }
                    catch
                    {
                        // 继续尝试下一个路径
                    }
                }

                if (configPath == null)
                {
                    Debug.LogWarning($"[RandomPresetGenerator] 未找到预设范围配置文件，使用默认范围");
                    return GenerateDefaultRandomPreset();
                }

                string jsonContent = File.ReadAllText(configPath, Encoding.UTF8);
                return ParseAndGeneratePreset(jsonContent);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[RandomPresetGenerator] 生成随机preset时发生错误: {ex.Message}");
                Debug.LogError($"[RandomPresetGenerator] 堆栈跟踪: {ex.StackTrace}");
                return GenerateDefaultRandomPreset();
            }
        }

        /// <summary>
        /// 解析JSON配置并生成随机preset
        /// </summary>
        private static string ParseAndGeneratePreset(string jsonContent)
        {
            // 使用简单的字符串解析来读取范围
            // 因为Unity可能没有完整的JSON库支持
            
            var preset = new Dictionary<string, object>();
            preset["savedSetting"] = false;

            // 解析headSetting
            var headSetting = new Dictionary<string, object>();
            var mainColor = new Dictionary<string, object>();
            mainColor["r"] = GetRandomColorValueFromRange(jsonContent, "headSetting", "mainColor", "r", 0f, 1f);
            mainColor["g"] = GetRandomColorValueFromRange(jsonContent, "headSetting", "mainColor", "g", 0f, 1f);
            mainColor["b"] = GetRandomColorValueFromRange(jsonContent, "headSetting", "mainColor", "b", 0f, 1f);
            mainColor["a"] = 1f;
            headSetting["mainColor"] = mainColor;
            headSetting["headScaleOffset"] = GetRandomValueFromRange(jsonContent, "headSetting", "headScaleOffset", -0.4f, 0.4f);
            headSetting["foreheadHeight"] = GetRandomValueFromRange(jsonContent, "headSetting", "foreheadHeight", 0f, 6f);
            headSetting["foreheadRound"] = GetRandomValueFromRange(jsonContent, "headSetting", "foreheadRound", 0.35f, 1f);
            preset["headSetting"] = headSetting;

            // 解析ID范围（字符串格式如 "0-16"）
            preset["hairID"] = GetRandomIDFromRange(jsonContent, "hairID", 0, 16);
            preset["eyeID"] = GetRandomIDFromRange(jsonContent, "eyeID", 0, 14);
            preset["eyebrowID"] = GetRandomIDFromRange(jsonContent, "eyebrowID", 0, 8);
            preset["mouthID"] = GetRandomIDFromRange(jsonContent, "mouthID", 0, 31);
            preset["tailID"] = GetRandomIDFromRange(jsonContent, "tailID", 0, 4);
            preset["footID"] = GetRandomIDFromRange(jsonContent, "footID", 0, 3);
            preset["wingID"] = GetRandomIDFromRange(jsonContent, "wingID", 0, 3);

            // 解析各个Info部分
            preset["hairInfo"] = ParseInfo(jsonContent, "hairInfo", new Dictionary<string, float[]>
            {
                { "radius", new[] { 0f } },
                { "height", new[] { 0f } },
                { "heightOffset", new[] { 0f } },
                { "scale", new[] { 1f } },
                { "twist", new[] { 0f } },
                { "distanceAngle", new[] { 0f } },
                { "leftRightAngle", new[] { 0f } }
            });
            preset["eyeInfo"] = ParseInfo(jsonContent, "eyeInfo", new Dictionary<string, float[]>
            {
                { "radius", new[] { 0.23f } },
                { "height", new[] { -0.3f, 0.3f } },
                { "heightOffset", new[] { 0f } },
                { "scale", new[] { 0.3f, 4f } },
                { "twist", new[] { -90f, 90f } },
                { "distanceAngle", new[] { 0f, 90f } },
                { "leftRightAngle", new[] { 0f } }
            });
            preset["eyebrowInfo"] = ParseInfo(jsonContent, "eyebrowInfo", new Dictionary<string, float[]>
            {
                { "radius", new[] { 0.23f } },
                { "height", new[] { -0.3f, 0.3f } },
                { "heightOffset", new[] { -0.3f, 0.3f } },
                { "scale", new[] { 0.3f, 4f } },
                { "twist", new[] { -90f, 90f } },
                { "distanceAngle", new[] { 0f, 90f } },
                { "leftRightAngle", new[] { 0f } }
            });
            preset["mouthInfo"] = ParseInfo(jsonContent, "mouthInfo", new Dictionary<string, float[]>
            {
                { "radius", new[] { 0.23f } },
                { "height", new[] { -0.3f, 0.3f } },
                { "heightOffset", new[] { 0f } },
                { "scale", new[] { 0.3f, 4f } },
                { "twist", new[] { -90f, 90f } },
                { "distanceAngle", new[] { 0f } },
                { "leftRightAngle", new[] { -50f, 50f } }
            });
            preset["tailInfo"] = ParseInfo(jsonContent, "tailInfo", new Dictionary<string, float[]>
            {
                { "radius", new[] { 0f } },
                { "height", new[] { 0f } },
                { "heightOffset", new[] { 0f } },
                { "scale", new[] { 0.3f, 2f } },
                { "twist", new[] { 0f } },
                { "distanceAngle", new[] { 0f } },
                { "leftRightAngle", new[] { 0f } }
            });
            preset["footInfo"] = ParseInfo(jsonContent, "footInfo", new Dictionary<string, float[]>
            {
                { "radius", new[] { 0f } },
                { "height", new[] { 0f } },
                { "heightOffset", new[] { 0f } },
                { "scale", new[] { 0.5f, 1.5f } },
                { "twist", new[] { 0f } },
                { "distanceAngle", new[] { 0f } },
                { "leftRightAngle", new[] { 0f } }
            });
            // wingInfo的color在JSON中是固定值(0,0,0)，不是范围
            var wingInfo = new Dictionary<string, object>();
            var wingColor = new Dictionary<string, object>();
            wingColor["r"] = 0f;
            wingColor["g"] = 0f;
            wingColor["b"] = 0f;
            wingColor["a"] = 1f;
            wingInfo["color"] = wingColor;
            wingInfo["radius"] = 0f;
            wingInfo["height"] = 0f;
            wingInfo["heightOffset"] = 0f;
            wingInfo["scale"] = GetRandomValueFromRange(jsonContent, "wingInfo", "scale", 0.5f, 2f);
            wingInfo["twist"] = 0f;
            wingInfo["distanceAngle"] = 0f;
            wingInfo["leftRightAngle"] = 0f;
            preset["wingInfo"] = wingInfo;

            // 转换为JSON字符串
            return ConvertToJsonString(preset);
        }

        /// <summary>
        /// 解析Info对象
        /// </summary>
        private static Dictionary<string, object> ParseInfo(string jsonContent, string infoName, Dictionary<string, float[]> defaultRanges)
        {
            var info = new Dictionary<string, object>();
            
            // 颜色总是随机
            var color = new Dictionary<string, object>();
            color["r"] = GetRandomColorValueFromRange(jsonContent, infoName, "color", "r", 0f, 1f);
            color["g"] = GetRandomColorValueFromRange(jsonContent, infoName, "color", "g", 0f, 1f);
            color["b"] = GetRandomColorValueFromRange(jsonContent, infoName, "color", "b", 0f, 1f);
            color["a"] = 1f;
            info["color"] = color;

            // 解析其他字段
            foreach (var kvp in defaultRanges)
            {
                string fieldName = kvp.Key;
                float[] range = kvp.Value;
                
                if (range.Length == 1)
                {
                    info[fieldName] = range[0];
                }
                else if (range.Length == 2)
                {
                    info[fieldName] = GetRandomValueFromRange(jsonContent, infoName, fieldName, range[0], range[1]);
                }
            }

            return info;
        }

        /// <summary>
        /// 从JSON中提取范围值（简化版解析）
        /// </summary>
        private static float GetRandomValueFromRange(string jsonContent, string section, string field, float minDefault, float maxDefault)
        {
            // 尝试从JSON中解析范围，如果失败则使用默认值
            try
            {
                // 查找字段的模式，例如 "field": [min, max] 或 "field": [min, min, max]
                // 先尝试在section内查找
                string pattern = $"\"{section}\":\\s*\\{{[^}}]*\"{field}\":\\s*\\[([^\\]]+)\\]";
                var match = System.Text.RegularExpressions.Regex.Match(jsonContent, pattern);
                if (!match.Success)
                {
                    // 如果没找到，尝试直接查找字段
                    pattern = $"\"{field}\":\\s*\\[([^\\]]+)\\]";
                    match = System.Text.RegularExpressions.Regex.Match(jsonContent, pattern);
                }
                
                if (match.Success)
                {
                    string rangeStr = match.Groups[1].Value;
                    string[] parts = rangeStr.Split(',');
                    if (parts.Length >= 2)
                    {
                        float min, max;
                        
                        // 如果数组有3个元素（如[0, 0, 6]），使用第一个和最后一个
                        if (parts.Length >= 3)
                        {
                            if (float.TryParse(parts[0].Trim(), out min) && 
                                float.TryParse(parts[parts.Length - 1].Trim(), out max))
                            {
                                return (float)(_random.NextDouble() * (max - min) + min);
                            }
                        }
                        // 如果数组有2个元素（如[0, 6]），使用这两个值
                        else if (parts.Length == 2)
                        {
                            if (float.TryParse(parts[0].Trim(), out min) && 
                                float.TryParse(parts[1].Trim(), out max))
                            {
                                return (float)(_random.NextDouble() * (max - min) + min);
                            }
                        }
                    }
                }
            }
            catch
            {
                // 解析失败，使用默认值
            }
            
            return (float)(_random.NextDouble() * (maxDefault - minDefault) + minDefault);
        }

        /// <summary>
        /// 从JSON中提取颜色字段的范围值（嵌套结构，如mainColor.r）
        /// </summary>
        private static float GetRandomColorValueFromRange(string jsonContent, string section, string colorField, string channel, float minDefault, float maxDefault)
        {
            // 尝试从JSON中解析范围，如果失败则使用默认值
            try
            {
                // 查找嵌套颜色字段的模式，例如 "color": { "r": [0, 1] }
                string pattern = $"\"{section}\":\\s*\\{{[^}}]*\"{colorField}\":\\s*\\{{[^}}]*\"{channel}\":\\s*\\[([^\\]]+)\\]";
                var match = System.Text.RegularExpressions.Regex.Match(jsonContent, pattern);
                if (!match.Success)
                {
                    // 如果没找到，尝试直接查找color.channel
                    pattern = $"\"{colorField}\":\\s*\\{{[^}}]*\"{channel}\":\\s*\\[([^\\]]+)\\]";
                    match = System.Text.RegularExpressions.Regex.Match(jsonContent, pattern);
                }
                
                if (match.Success)
                {
                    string rangeStr = match.Groups[1].Value;
                    string[] parts = rangeStr.Split(',');
                    if (parts.Length >= 2)
                    {
                        float min, max;
                        
                        // 如果数组有3个元素，使用第一个和最后一个
                        if (parts.Length >= 3)
                        {
                            if (float.TryParse(parts[0].Trim(), out min) && 
                                float.TryParse(parts[parts.Length - 1].Trim(), out max))
                            {
                                return (float)(_random.NextDouble() * (max - min) + min);
                            }
                        }
                        // 如果数组有2个元素，使用这两个值
                        else if (parts.Length == 2)
                        {
                            if (float.TryParse(parts[0].Trim(), out min) && 
                                float.TryParse(parts[1].Trim(), out max))
                            {
                                return (float)(_random.NextDouble() * (max - min) + min);
                            }
                        }
                    }
                }
            }
            catch
            {
                // 解析失败，使用默认值
            }
            
            return (float)(_random.NextDouble() * (maxDefault - minDefault) + minDefault);
        }

        /// <summary>
        /// 从JSON中提取ID范围
        /// </summary>
        private static int GetRandomIDFromRange(string jsonContent, string fieldName, int minDefault, int maxDefault)
        {
            try
            {
                // 查找字段的模式，例如 "fieldID": "0-16"
                string pattern = $"\"{fieldName}\":\\s*\"([0-9]+)-([0-9]+)\"";
                var match = System.Text.RegularExpressions.Regex.Match(jsonContent, pattern);
                if (match.Success)
                {
                    if (int.TryParse(match.Groups[1].Value, out int min) && 
                        int.TryParse(match.Groups[2].Value, out int max))
                    {
                        return _random.Next(min, max + 1);
                    }
                }
            }
            catch
            {
                // 解析失败，使用默认值
            }
            
            return _random.Next(minDefault, maxDefault + 1);
        }

        /// <summary>
        /// 将字典转换为JSON字符串
        /// </summary>
        private static string ConvertToJsonString(Dictionary<string, object> dict)
        {
            var sb = new StringBuilder();
            sb.Append("{");
            bool first = true;
            
            foreach (var kvp in dict)
            {
                if (!first) sb.Append(",");
                first = false;
                
                sb.Append($"\"{kvp.Key}\":");
                
                if (kvp.Value is Dictionary<string, object> nestedDict)
                {
                    sb.Append(ConvertToJsonString(nestedDict));
                }
                else if (kvp.Value is float f)
                {
                    sb.Append(f.ToString("F3", System.Globalization.CultureInfo.InvariantCulture));
                }
                else if (kvp.Value is double d)
                {
                    sb.Append(d.ToString("F3", System.Globalization.CultureInfo.InvariantCulture));
                }
                else if (kvp.Value is int)
                {
                    sb.Append(kvp.Value.ToString());
                }
                else if (kvp.Value is bool)
                {
                    sb.Append(kvp.Value.ToString().ToLower());
                }
                else
                {
                    sb.Append($"\"{kvp.Value}\"");
                }
            }
            
            sb.Append("}");
            return sb.ToString();
        }

        /// <summary>
        /// 生成默认随机preset（当配置文件不存在时）
        /// </summary>
        private static string GenerateDefaultRandomPreset()
        {
            var preset = new Dictionary<string, object>();
            preset["savedSetting"] = false;

            // 头部设置
            var headSetting = new Dictionary<string, object>();
            var mainColor = new Dictionary<string, object>();
            mainColor["r"] = (float)_random.NextDouble();
            mainColor["g"] = (float)_random.NextDouble();
            mainColor["b"] = (float)_random.NextDouble();
            mainColor["a"] = 1f;
            headSetting["mainColor"] = mainColor;
            headSetting["headScaleOffset"] = (float)(_random.NextDouble() * 0.8 - 0.4);
            headSetting["foreheadHeight"] = (float)(_random.NextDouble() * 6);
            headSetting["foreheadRound"] = (float)(_random.NextDouble() * 0.65 + 0.35);
            preset["headSetting"] = headSetting;

            // ID
            preset["hairID"] = _random.Next(0, 17);
            preset["eyeID"] = _random.Next(0, 15);
            preset["eyebrowID"] = _random.Next(0, 9);
            preset["mouthID"] = _random.Next(0, 32);
            preset["tailID"] = _random.Next(0, 5);
            preset["footID"] = _random.Next(0, 4);
            preset["wingID"] = _random.Next(0, 4);

            // 各个Info部分
            preset["hairInfo"] = CreateRandomInfo(new Dictionary<string, float[]>
            {
                { "radius", new[] { 0f } },
                { "height", new[] { 0f } },
                { "heightOffset", new[] { 0f } },
                { "scale", new[] { 1f } },
                { "twist", new[] { 0f } },
                { "distanceAngle", new[] { 0f } },
                { "leftRightAngle", new[] { 0f } }
            });
            preset["eyeInfo"] = CreateRandomInfo(new Dictionary<string, float[]>
            {
                { "radius", new[] { 0.23f } },
                { "height", new[] { -0.3f, 0.3f } },
                { "heightOffset", new[] { 0f } },
                { "scale", new[] { 0.3f, 4f } },
                { "twist", new[] { -90f, 90f } },
                { "distanceAngle", new[] { 0f, 90f } },
                { "leftRightAngle", new[] { 0f } }
            });
            preset["eyebrowInfo"] = CreateRandomInfo(new Dictionary<string, float[]>
            {
                { "radius", new[] { 0.23f } },
                { "height", new[] { -0.3f, 0.3f } },
                { "heightOffset", new[] { -0.3f, 0.3f } },
                { "scale", new[] { 0.3f, 4f } },
                { "twist", new[] { -90f, 90f } },
                { "distanceAngle", new[] { 0f, 90f } },
                { "leftRightAngle", new[] { 0f } }
            });
            preset["mouthInfo"] = CreateRandomInfo(new Dictionary<string, float[]>
            {
                { "radius", new[] { 0.23f } },
                { "height", new[] { -0.3f, 0.3f } },
                { "heightOffset", new[] { 0f } },
                { "scale", new[] { 0.3f, 4f } },
                { "twist", new[] { -90f, 90f } },
                { "distanceAngle", new[] { 0f } },
                { "leftRightAngle", new[] { -50f, 50f } }
            });
            preset["tailInfo"] = CreateRandomInfo(new Dictionary<string, float[]>
            {
                { "radius", new[] { 0f } },
                { "height", new[] { 0f } },
                { "heightOffset", new[] { 0f } },
                { "scale", new[] { 0.3f, 2f } },
                { "twist", new[] { 0f } },
                { "distanceAngle", new[] { 0f } },
                { "leftRightAngle", new[] { 0f } }
            });
            preset["footInfo"] = CreateRandomInfo(new Dictionary<string, float[]>
            {
                { "radius", new[] { 0f } },
                { "height", new[] { 0f } },
                { "heightOffset", new[] { 0f } },
                { "scale", new[] { 0.5f, 1.5f } },
                { "twist", new[] { 0f } },
                { "distanceAngle", new[] { 0f } },
                { "leftRightAngle", new[] { 0f } }
            });
            // wingInfo的color在默认情况下是固定值(0,0,0)
            var wingInfo = new Dictionary<string, object>();
            var wingColor = new Dictionary<string, object>();
            wingColor["r"] = 0f;
            wingColor["g"] = 0f;
            wingColor["b"] = 0f;
            wingColor["a"] = 1f;
            wingInfo["color"] = wingColor;
            wingInfo["radius"] = 0f;
            wingInfo["height"] = 0f;
            wingInfo["heightOffset"] = 0f;
            wingInfo["scale"] = (float)(_random.NextDouble() * 1.5 + 0.5);
            wingInfo["twist"] = 0f;
            wingInfo["distanceAngle"] = 0f;
            wingInfo["leftRightAngle"] = 0f;
            preset["wingInfo"] = wingInfo;

            return ConvertToJsonString(preset);
        }

        /// <summary>
        /// 创建随机Info对象
        /// </summary>
        private static Dictionary<string, object> CreateRandomInfo(Dictionary<string, float[]> ranges)
        {
            var info = new Dictionary<string, object>();
            
            var color = new Dictionary<string, object>();
            color["r"] = (float)_random.NextDouble();
            color["g"] = (float)_random.NextDouble();
            color["b"] = (float)_random.NextDouble();
            color["a"] = 1f;
            info["color"] = color;

            foreach (var kvp in ranges)
            {
                if (kvp.Value.Length == 1)
                {
                    info[kvp.Key] = kvp.Value[0];
                }
                else if (kvp.Value.Length == 2)
                {
                    info[kvp.Key] = (float)(_random.NextDouble() * (kvp.Value[1] - kvp.Value[0]) + kvp.Value[0]);
                }
            }

            return info;
        }
    }
}

