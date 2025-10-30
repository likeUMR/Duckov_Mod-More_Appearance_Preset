using System;
using UnityEngine;

namespace MoreAppearancePreset
{
    /// <summary>
    /// UI查找和路径查找工具类
    /// </summary>
    public static class UIFinder
    {
        /// <summary>
        /// 通过路径查找GameObject，包括未激活的对象
        /// </summary>
        public static GameObject? FindGameObjectByPath(string path)
        {
            try
            {
                // 分割路径
                string[] pathParts = path.Split('/');
                
                if (pathParts.Length == 0)
                {
                    return null;
                }

                // 从根对象开始查找（尝试查找CustomFace）
                GameObject? rootObject = GameObject.Find(pathParts[0]);
                
                if (rootObject == null)
                {
                    // 如果找不到，尝试在所有场景对象中查找（包括未激活的）
                    rootObject = FindInAllObjects(pathParts[0]);
                }

                if (rootObject == null)
                {
                    return null;
                }

                // 沿着路径向下查找
                Transform current = rootObject.transform;
                for (int i = 1; i < pathParts.Length; i++)
                {
                    // 使用Find方法查找子对象（即使未激活也能找到）
                    Transform? child = current.Find(pathParts[i]);
                    
                    if (child == null)
                    {
                        // 如果Find找不到，尝试遍历所有子对象查找（包括未激活的）
                        child = FindChildByName(current, pathParts[i]);
                    }

                    if (child == null)
                    {
                        return null;
                    }

                    current = child;
                }

                return current.gameObject;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[MoreAppearancePreset] 查找GameObject路径时发生错误: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 在所有场景对象中查找指定名称的对象（包括未激活的）
        /// </summary>
        private static GameObject? FindInAllObjects(string name)
        {
            // 获取所有场景中的根对象
            GameObject[] rootObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
            
            foreach (GameObject root in rootObjects)
            {
                if (root.name == name)
                {
                    return root;
                }

                // 递归查找子对象
                Transform? found = FindInChildren(root.transform, name);
                if (found != null)
                {
                    return found.gameObject;
                }
            }

            return null;
        }

        /// <summary>
        /// 在Transform的子对象中查找指定名称的对象（包括未激活的）
        /// </summary>
        private static Transform? FindInChildren(Transform parent, string name)
        {
            foreach (Transform child in parent)
            {
                if (child.name == name)
                {
                    return child;
                }

                // 递归查找
                Transform? found = FindInChildren(child, name);
                if (found != null)
                {
                    return found;
                }
            }

            return null;
        }

        /// <summary>
        /// 在Transform的直接子对象中查找指定名称（包括未激活的）
        /// </summary>
        private static Transform? FindChildByName(Transform parent, string name)
        {
            foreach (Transform child in parent)
            {
                if (child.name == name)
                {
                    return child;
                }
            }

            return null;
        }

        /// <summary>
        /// 获取Transform的完整路径
        /// </summary>
        public static string GetFullPath(Transform transform)
        {
            string path = transform.name;
            Transform current = transform.parent;
            
            while (current != null)
            {
                path = current.name + "/" + path;
                current = current.parent;
            }
            
            return path;
        }
    }
}

