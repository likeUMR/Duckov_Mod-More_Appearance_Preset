using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MoreAppearancePreset
{
    /// <summary>
    /// Scrollbar查找工具 - 帮助查找游戏中所有Scrollbar的路径
    /// 使用方法：在ModBehaviour中调用 ScrollbarFinder.FindAndLogAllScrollbars()
    /// </summary>
    public static class ScrollbarFinder
    {
        /// <summary>
        /// 查找并输出游戏中所有Scrollbar的详细信息
        /// 调用时机：在捏脸场景中，Panels激活后
        /// </summary>
        public static void FindAndLogAllScrollbars()
        {
            try
            {
                Debug.Log("========================================");
                Debug.Log("[ScrollbarFinder] 开始查找游戏中的所有Scrollbar...");
                Debug.Log("========================================");

                // 查找所有Scrollbar（包括未激活的）
                Scrollbar[] allScrollbars = UnityEngine.Object.FindObjectsOfType<Scrollbar>(true);

                if (allScrollbars == null || allScrollbars.Length == 0)
                {
                    Debug.LogWarning("[ScrollbarFinder] ✗ 场景中未找到任何Scrollbar组件");
                    return;
                }

                Debug.Log($"[ScrollbarFinder] ✓ 找到 {allScrollbars.Length} 个Scrollbar组件");
                Debug.Log("");

                // 逐个输出详细信息
                for (int i = 0; i < allScrollbars.Length; i++)
                {
                    Scrollbar scrollbar = allScrollbars[i];
                    LogScrollbarDetails(scrollbar, i + 1);
                }

                Debug.Log("========================================");
                Debug.Log("[ScrollbarFinder] Scrollbar查找完成");
                Debug.Log("========================================");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[ScrollbarFinder] 查找Scrollbar时发生错误: {ex.Message}");
                Debug.LogError($"[ScrollbarFinder] 堆栈跟踪: {ex.StackTrace}");
            }
        }

        /// <summary>
        /// 输出单个Scrollbar的详细信息
        /// </summary>
        private static void LogScrollbarDetails(Scrollbar scrollbar, int index)
        {
            Debug.Log($"===== Scrollbar [{index}] =====");
            Debug.Log($"名称: {scrollbar.name}");
            Debug.Log($"完整路径: {UIFinder.GetFullPath(scrollbar.transform)}");
            Debug.Log($"激活状态: {scrollbar.gameObject.activeInHierarchy}");
            Debug.Log($"方向: {scrollbar.direction}");

            // 输出ColorBlock
            Debug.Log($"ColorBlock:");
            Debug.Log($"  normalColor: {scrollbar.colors.normalColor}");
            Debug.Log($"  highlightedColor: {scrollbar.colors.highlightedColor}");
            Debug.Log($"  pressedColor: {scrollbar.colors.pressedColor}");
            Debug.Log($"  selectedColor: {scrollbar.colors.selectedColor}");
            Debug.Log($"  disabledColor: {scrollbar.colors.disabledColor}");

            // 输出背景Image信息
            Image? bgImage = scrollbar.GetComponent<Image>();
            if (bgImage != null)
            {
                Debug.Log($"背景Image:");
                Debug.Log($"  color: {bgImage.color}");
                Debug.Log($"  sprite: {bgImage.sprite?.name ?? "(null)"}");
                Debug.Log($"  type: {bgImage.type}");
            }

            // 输出Handle信息
            if (scrollbar.handleRect != null)
            {
                Debug.Log($"Handle:");
                Debug.Log($"  名称: {scrollbar.handleRect.name}");

                Image? handleImage = scrollbar.handleRect.GetComponent<Image>();
                if (handleImage != null)
                {
                    Debug.Log($"  color: {handleImage.color}");
                    Debug.Log($"  sprite: {handleImage.sprite?.name ?? "(null)"}");
                    Debug.Log($"  type: {handleImage.type}");
                }
            }

            Debug.Log("");
        }

        /// <summary>
        /// 根据特定条件查找Scrollbar
        /// </summary>
        public static Scrollbar? FindScrollbarByCondition(System.Func<Scrollbar, bool> condition)
        {
            try
            {
                Scrollbar[] allScrollbars = UnityEngine.Object.FindObjectsOfType<Scrollbar>(true);

                if (allScrollbars == null || allScrollbars.Length == 0)
                {
                    return null;
                }

                foreach (Scrollbar scrollbar in allScrollbars)
                {
                    if (condition(scrollbar))
                    {
                        return scrollbar;
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[ScrollbarFinder] 查找Scrollbar时发生错误: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 查找横向Scrollbar（如手大小调节）
        /// </summary>
        public static Scrollbar? FindHorizontalScrollbar()
        {
            return FindScrollbarByCondition(s =>
                s.direction == Scrollbar.Direction.LeftToRight ||
                s.direction == Scrollbar.Direction.RightToLeft);
        }

        /// <summary>
        /// 查找垂直Scrollbar
        /// </summary>
        public static Scrollbar? FindVerticalScrollbar()
        {
            return FindScrollbarByCondition(s =>
                s.direction == Scrollbar.Direction.BottomToTop ||
                s.direction == Scrollbar.Direction.TopToBottom);
        }

        /// <summary>
        /// 根据名称查找Scrollbar
        /// </summary>
        public static Scrollbar? FindScrollbarByName(string name)
        {
            return FindScrollbarByCondition(s => s.name.Contains(name));
        }

        /// <summary>
        /// 根据路径查找Scrollbar
        /// </summary>
        public static Scrollbar? FindScrollbarByPath(string pathKeyword)
        {
            return FindScrollbarByCondition(s =>
            {
                string fullPath = UIFinder.GetFullPath(s.transform);
                return fullPath.Contains(pathKeyword);
            });
        }
    }
}