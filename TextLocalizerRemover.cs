using System;
using System.Collections.Generic;
using UnityEngine;

namespace MoreAppearancePreset
{
    /// <summary>
    /// TextLocalizor组件移除工具类
    /// </summary>
    public static class TextLocalizerRemover
    {
        /// <summary>
        /// 移除GameObject及其所有子对象上的TextLocalizor组件
        /// </summary>
        public static void RemoveTextLocalizers(GameObject obj)
        {
            if (obj == null)
            {
                return;
            }

            try
            {
                Debug.Log($"[TextLocalizerRemover] 开始移除TextLocalizor组件，对象: {obj.name}");

                List<Component> componentsToRemove = new List<Component>();

                // 获取对象本身的所有组件
                Component[] allComponents = obj.GetComponents<Component>();
                Debug.Log($"[TextLocalizerRemover] 对象 {obj.name} 本身有 {allComponents.Length} 个组件");
                
                foreach (Component comp in allComponents)
                {
                    if (comp != null)
                    {
                        string componentTypeName = comp.GetType().Name;
                        Debug.Log($"[TextLocalizerRemover] 检查组件: {componentTypeName}");
                        
                        if (componentTypeName == "TextLocalizor")
                        {
                            componentsToRemove.Add(comp);
                            Debug.Log($"[TextLocalizerRemover] 标记待删除: {obj.name} 上的 {componentTypeName}");
                        }
                    }
                }

                // 获取所有子对象的组件（包括未激活的）
                Component[] allChildComponents = obj.GetComponentsInChildren<Component>(true);
                Debug.Log($"[TextLocalizerRemover] 对象 {obj.name} 及其子对象共有 {allChildComponents.Length} 个组件");
                
                foreach (Component comp in allChildComponents)
                {
                    if (comp != null)
                    {
                        string componentTypeName = comp.GetType().Name;
                        
                        if (componentTypeName == "TextLocalizor")
                        {
                            // 避免重复添加（如果已经在allComponents中）
                            if (!componentsToRemove.Contains(comp))
                            {
                                componentsToRemove.Add(comp);
                                Debug.Log($"[TextLocalizerRemover] 标记待删除: {comp.gameObject.name} 上的 {componentTypeName}");
                            }
                        }
                    }
                }

                // 移除找到的组件
                if (componentsToRemove.Count > 0)
                {
                    Debug.Log($"[TextLocalizerRemover] 准备移除 {componentsToRemove.Count} 个TextLocalizor组件");
                    
                    foreach (Component comp in componentsToRemove)
                    {
                        if (comp != null)
                        {
                            string objName = comp.gameObject.name;
                            UnityEngine.Object.DestroyImmediate(comp);
                            Debug.Log($"[TextLocalizerRemover] ✓ 已移除TextLocalizor组件: {objName}");
                        }
                    }
                    
                    Debug.Log($"[TextLocalizerRemover] ✓ 成功移除了 {componentsToRemove.Count} 个TextLocalizor组件");
                }
                else
                {
                    Debug.Log($"[TextLocalizerRemover] 未找到TextLocalizor组件");
                }

                Debug.Log($"[TextLocalizerRemover] ✓ 完成移除TextLocalizor组件，对象: {obj.name}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[MoreAppearancePreset] 移除TextLocalizor组件时发生错误: {ex.Message}");
                Debug.LogError($"[MoreAppearancePreset] 堆栈跟踪: {ex.StackTrace}");
            }
        }

        /// <summary>
        /// 移除特定Transform及其子对象上的TextLocalizor组件
        /// </summary>
        public static void RemoveTextLocalizers(Transform transform)
        {
            if (transform != null)
            {
                RemoveTextLocalizers(transform.gameObject);
            }
        }
    }
}



