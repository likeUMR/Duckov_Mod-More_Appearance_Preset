using System;
using UnityEngine;
using UnityEngine.UI;

namespace MoreAppearancePreset
{
    /// <summary>
    /// ScrollView创建器 - 复制游戏Dropdown中的Scrollbar
    /// </summary>
    public static class ScrollViewCreator
    {
        // ===== 可调整的参数 =====
        private const float SCROLLVIEW_WIDTH = 350f;  // 宽度
        private const float SCROLLVIEW_HEIGHT = 655f; // 高度
        private const float SCROLLBAR_WIDTH = 15f;
        private const float CONTENT_WIDTH = SCROLLVIEW_WIDTH - SCROLLBAR_WIDTH;

        private static readonly RectOffset LAYOUT_PADDING = new RectOffset(0, 0, 0, 0);
        private const float LAYOUT_SPACING = 10f;

        // 【修改】使用Dropdown模板中的Scrollbar（蓝灰色轨道+白色滑块）
        private const string GAME_SCROLLBAR_PATH = "GameManager (from Startup)/PauseMenu/Menu/OptionsPanel/ScrollView/Viewport/Content/Graphics/UI_Shadow/Dropdown/Template/Scrollbar";

        /// <summary>
        /// 为Preset面板创建ScrollView结构
        /// </summary>
        public static GameObject? CreateScrollView(GameObject presetPanel)
        {
            if (presetPanel == null)
            {
                Debug.LogError("[ScrollViewCreator] Preset面板为null");
                return null;
            }

            try
            {
                Debug.Log("[ScrollViewCreator] ===== 开始创建ScrollView（复制Dropdown的Scrollbar） =====");

                GameObject scrollViewObj = CreateScrollViewContainer(presetPanel);
                RectTransform scrollViewRect = scrollViewObj.GetComponent<RectTransform>();

                GameObject viewportObj = CreateViewport(scrollViewObj);
                RectTransform viewportRect = viewportObj.GetComponent<RectTransform>();

                GameObject contentObj = CreateContent(viewportObj);
                RectTransform contentRect = contentObj.GetComponent<RectTransform>();

                GameObject? scrollbarObj = CreateScrollbarFromGame(scrollViewObj);

                ConfigureScrollRect(scrollViewObj, contentRect, viewportRect, scrollbarObj);

                Canvas.ForceUpdateCanvases();
                LayoutRebuilder.ForceRebuildLayoutImmediate(contentRect);

                scrollViewObj.SetActive(true);
                viewportObj.SetActive(true);
                contentObj.SetActive(true);
                if (scrollbarObj != null) scrollbarObj.SetActive(true);

                LogScrollViewInfo(scrollViewRect, viewportRect, contentRect);

                Debug.Log($"[ScrollViewCreator] ===== ScrollView创建完成 =====");
                return contentObj;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[ScrollViewCreator] 创建ScrollView时发生错误: {ex.Message}");
                Debug.LogError($"[ScrollViewCreator] 堆栈跟踪: {ex.StackTrace}");
                return null;
            }
        }

        private static GameObject CreateScrollViewContainer(GameObject parent)
        {
            Debug.Log("[ScrollViewCreator] 创建ScrollView容器...");

            GameObject scrollViewObj = new GameObject("ScrollView");
            scrollViewObj.transform.SetParent(parent.transform, false);
            scrollViewObj.layer = LayerMask.NameToLayer("UI");

            RectTransform rect = scrollViewObj.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(0, 1);
            rect.pivot = new Vector2(0, 1);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(SCROLLVIEW_WIDTH, SCROLLVIEW_HEIGHT);

            Debug.Log($"[ScrollViewCreator] ✓ ScrollView容器创建完成 (宽度={SCROLLVIEW_WIDTH}, 高度={SCROLLVIEW_HEIGHT})");
            return scrollViewObj;
        }

        private static GameObject CreateViewport(GameObject parent)
        {
            Debug.Log("[ScrollViewCreator] 创建Viewport...");

            GameObject viewportObj = new GameObject("Viewport");
            viewportObj.transform.SetParent(parent.transform, false);
            viewportObj.layer = LayerMask.NameToLayer("UI");

            RectTransform rect = viewportObj.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 0);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(0, 1);
            rect.anchoredPosition = Vector2.zero;
            rect.offsetMin = new Vector2(0, 0);
            rect.offsetMax = new Vector2(-SCROLLBAR_WIDTH, 0);

            Image image = viewportObj.AddComponent<Image>();
            image.color = new Color(1f, 1f, 1f, 0.01f);
            image.raycastTarget = true;

            Mask mask = viewportObj.AddComponent<Mask>();
            mask.showMaskGraphic = false;

            Debug.Log($"[ScrollViewCreator] ✓ Viewport创建完成 (为Scrollbar预留宽度={SCROLLBAR_WIDTH})");
            return viewportObj;
        }

        private static GameObject CreateContent(GameObject parent)
        {
            Debug.Log("[ScrollViewCreator] 创建Content...");

            GameObject contentObj = new GameObject("Content");
            contentObj.transform.SetParent(parent.transform, false);
            contentObj.layer = LayerMask.NameToLayer("UI");

            RectTransform rect = contentObj.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(0, 1);
            rect.pivot = new Vector2(0, 1);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(CONTENT_WIDTH, 100f);

            VerticalLayoutGroup layoutGroup = contentObj.AddComponent<VerticalLayoutGroup>();
            layoutGroup.padding = LAYOUT_PADDING;
            layoutGroup.spacing = LAYOUT_SPACING;
            layoutGroup.childAlignment = TextAnchor.UpperLeft;
            layoutGroup.childControlWidth = true;
            layoutGroup.childControlHeight = true;
            layoutGroup.childForceExpandWidth = false;
            layoutGroup.childForceExpandHeight = false;
            layoutGroup.childScaleWidth = true;
            layoutGroup.childScaleHeight = true;

            ContentSizeFitter sizeFitter = contentObj.AddComponent<ContentSizeFitter>();
            sizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            Debug.Log($"[ScrollViewCreator] ✓ Content创建完成 (内容区宽度={CONTENT_WIDTH})");
            return contentObj;
        }

        /// <summary>
        /// 【核心方法】从Dropdown模板中复制Scrollbar
        /// </summary>
        private static GameObject? CreateScrollbarFromGame(GameObject parent)
        {
            try
            {
                Debug.Log("[ScrollViewCreator] ===== 开始复制Dropdown的Scrollbar =====");
                Debug.Log($"[ScrollViewCreator] 源路径: {GAME_SCROLLBAR_PATH}");
                Debug.Log($"[ScrollViewCreator] 特点: 蓝灰色轨道(0.412,0.655,0.769) + 白色滑块");

                // 1. 查找源Scrollbar
                GameObject? sourceScrollbar = UIFinder.FindGameObjectByPath(GAME_SCROLLBAR_PATH);

                if (sourceScrollbar == null)
                {
                    Debug.LogError($"[ScrollViewCreator] ✗ 未找到源Scrollbar");
                    return null;
                }

                Debug.Log($"[ScrollViewCreator] ✓ 找到源Scrollbar: {sourceScrollbar.name}");

                // 2. 保存源Scrollbar的所有关键信息
                Scrollbar sourceScrollbarComp = sourceScrollbar.GetComponent<Scrollbar>();
                if (sourceScrollbarComp == null)
                {
                    Debug.LogError("[ScrollViewCreator] ✗ 源对象没有Scrollbar组件");
                    return null;
                }

                // 保存ColorBlock
                ColorBlock sourceColors = sourceScrollbarComp.colors;
                Debug.Log($"[ScrollViewCreator] ✓ 源ColorBlock - normal:{sourceColors.normalColor}, highlighted:{sourceColors.highlightedColor}");

                // 保存背景Image信息
                Image? sourceBackgroundImage = sourceScrollbar.GetComponent<Image>();
                Sprite? sourceBgSprite = null;
                Color sourceBgColor = Color.white;
                Image.Type sourceBgType = Image.Type.Simple;
                Material? sourceBgMaterial = null;

                if (sourceBackgroundImage != null)
                {
                    sourceBgSprite = sourceBackgroundImage.sprite;
                    sourceBgColor = sourceBackgroundImage.color;
                    sourceBgType = sourceBackgroundImage.type;
                    sourceBgMaterial = sourceBackgroundImage.material;
                    Debug.Log($"[ScrollViewCreator] ✓ 源背景 - 颜色:{sourceBgColor}, Sprite:{sourceBgSprite?.name}, Type:{sourceBgType}");
                }

                // 保存Handle信息
                RectTransform? sourceHandleRect = sourceScrollbarComp.handleRect;
                Sprite? sourceHandleSprite = null;
                Color sourceHandleColor = Color.white;
                Image.Type sourceHandleType = Image.Type.Simple;
                Material? sourceHandleMaterial = null;
                Vector2 sourceHandleAnchorMin = Vector2.zero;
                Vector2 sourceHandleAnchorMax = Vector2.one;
                Vector2 sourceHandlePivot = new Vector2(0.5f, 0.5f);
                Vector2 sourceHandleOffsetMin = Vector2.zero;
                Vector2 sourceHandleOffsetMax = Vector2.zero;
                Vector2 sourceHandleSizeDelta = Vector2.zero;

                if (sourceHandleRect != null)
                {
                    Image? sourceHandleImage = sourceHandleRect.GetComponent<Image>();
                    if (sourceHandleImage != null)
                    {
                        sourceHandleSprite = sourceHandleImage.sprite;
                        sourceHandleColor = sourceHandleImage.color;
                        sourceHandleType = sourceHandleImage.type;
                        sourceHandleMaterial = sourceHandleImage.material;
                        Debug.Log($"[ScrollViewCreator] ✓ 源Handle - 颜色:{sourceHandleColor}, Sprite:{sourceHandleSprite?.name}, Type:{sourceHandleType}");
                    }

                    sourceHandleAnchorMin = sourceHandleRect.anchorMin;
                    sourceHandleAnchorMax = sourceHandleRect.anchorMax;
                    sourceHandlePivot = sourceHandleRect.pivot;
                    sourceHandleOffsetMin = sourceHandleRect.offsetMin;
                    sourceHandleOffsetMax = sourceHandleRect.offsetMax;
                    sourceHandleSizeDelta = sourceHandleRect.sizeDelta;
                    Debug.Log($"[ScrollViewCreator] ✓ 源Handle RectTransform - Anchor:[{sourceHandleAnchorMin},{sourceHandleAnchorMax}]");
                    Debug.Log($"[ScrollViewCreator]   Offset:[{sourceHandleOffsetMin},{sourceHandleOffsetMax}], SizeDelta:{sourceHandleSizeDelta}");
                }

                // 3. 复制GameObject
                GameObject scrollbarObj = UnityEngine.Object.Instantiate(sourceScrollbar, parent.transform);
                scrollbarObj.name = "Scrollbar Vertical";
                SetLayerRecursively(scrollbarObj, LayerMask.NameToLayer("UI"));

                Debug.Log("[ScrollViewCreator] ✓ 已复制Scrollbar对象");

                // 4. 获取复制后的组件
                Scrollbar scrollbar = scrollbarObj.GetComponent<Scrollbar>();
                Image? scrollbarImage = scrollbarObj.GetComponent<Image>();
                RectTransform scrollbarRect = scrollbarObj.GetComponent<RectTransform>();

                if (scrollbar == null || scrollbarImage == null || scrollbarRect == null)
                {
                    Debug.LogError("[ScrollViewCreator] ✗ 复制后的Scrollbar缺少必要组件");
                    return null;
                }

                // 5. 【关键】重新设置背景Image的所有属性
                scrollbarImage.sprite = sourceBgSprite;
                scrollbarImage.color = sourceBgColor;
                scrollbarImage.type = sourceBgType;
                scrollbarImage.raycastTarget = true;
                scrollbarImage.enabled = true;
                if (sourceBgMaterial != null)
                {
                    scrollbarImage.material = sourceBgMaterial;
                }

                Debug.Log($"[ScrollViewCreator] ✓ 已设置背景 - Sprite:{scrollbarImage.sprite?.name}, 颜色:{scrollbarImage.color}, Type:{scrollbarImage.type}");

                // 6. 设置Scrollbar的位置和大小
                scrollbarRect.anchorMin = new Vector2(1, 0);
                scrollbarRect.anchorMax = new Vector2(1, 1);
                scrollbarRect.pivot = new Vector2(1, 0.5f);
                scrollbarRect.anchoredPosition = Vector2.zero;
                scrollbarRect.sizeDelta = new Vector2(SCROLLBAR_WIDTH, 0);

                Debug.Log($"[ScrollViewCreator] ✓ 已设置Scrollbar位置和尺寸 - 宽度:{SCROLLBAR_WIDTH}");

                // 7. 【关键】设置Scrollbar组件属性
                scrollbar.direction = Scrollbar.Direction.BottomToTop;
                scrollbar.value = 1f;
                scrollbar.size = 0.2f; // 设置一个初始size，这决定了Handle的可见性
                scrollbar.numberOfSteps = 0;
                scrollbar.colors = sourceColors;
                scrollbar.transition = Selectable.Transition.ColorTint;
                scrollbar.interactable = true;

                Debug.Log($"[ScrollViewCreator] ✓ 已设置Scrollbar组件 - direction:{scrollbar.direction}, value:{scrollbar.value}, size:{scrollbar.size}");

                // 8. 【最关键】重新设置Handle的所有属性
                if (scrollbar.handleRect != null)
                {
                    RectTransform handleRect = scrollbar.handleRect;
                    Image? handleImage = handleRect.GetComponent<Image>();

                    if (handleImage != null)
                    {
                        handleImage.sprite = sourceHandleSprite;
                        handleImage.color = sourceHandleColor;
                        handleImage.type = sourceHandleType;
                        handleImage.raycastTarget = true;
                        handleImage.enabled = true;
                        if (sourceHandleMaterial != null)
                        {
                            handleImage.material = sourceHandleMaterial;
                        }

                        Debug.Log($"[ScrollViewCreator] ✓ 已设置Handle Image - Sprite:{handleImage.sprite?.name}, 颜色:{handleImage.color}, Type:{handleImage.type}");
                        Debug.Log($"[ScrollViewCreator]   Handle Image enabled:{handleImage.enabled}, raycastTarget:{handleImage.raycastTarget}");
                    }

                    // 恢复Handle的RectTransform设置
                    handleRect.anchorMin = sourceHandleAnchorMin;
                    handleRect.anchorMax = sourceHandleAnchorMax;
                    handleRect.pivot = sourceHandlePivot;
                    handleRect.offsetMin = sourceHandleOffsetMin;
                    handleRect.offsetMax = sourceHandleOffsetMax;

                    Debug.Log($"[ScrollViewCreator] ✓ 已设置Handle RectTransform");
                    Debug.Log($"[ScrollViewCreator]   Anchor:[{handleRect.anchorMin},{handleRect.anchorMax}]");
                    Debug.Log($"[ScrollViewCreator]   Offset:[{handleRect.offsetMin},{handleRect.offsetMax}]");

                    // 确保Handle激活
                    handleRect.gameObject.SetActive(true);
                    Debug.Log($"[ScrollViewCreator] ✓ Handle激活状态:{handleRect.gameObject.activeSelf}");
                }
                else
                {
                    Debug.LogError("[ScrollViewCreator] ✗ Handle不存在！");
                }

                // 9. 确保激活
                scrollbarObj.SetActive(true);

                // 10. 强制刷新
                Canvas.ForceUpdateCanvases();

                // 11. 最终验证
                Debug.Log("[ScrollViewCreator] ===== 最终验证 =====");
                Debug.Log($"[ScrollViewCreator] Scrollbar激活:{scrollbarObj.activeSelf}, Layer:{scrollbarObj.layer}");
                Debug.Log($"[ScrollViewCreator] Scrollbar背景Image enabled:{scrollbarImage.enabled}");

                if (scrollbar.handleRect != null)
                {
                    Debug.Log($"[ScrollViewCreator] Handle激活:{scrollbar.handleRect.gameObject.activeSelf}, Layer:{scrollbar.handleRect.gameObject.layer}");
                    Image? finalHandleImage = scrollbar.handleRect.GetComponent<Image>();
                    if (finalHandleImage != null)
                    {
                        Debug.Log($"[ScrollViewCreator] Handle Image enabled:{finalHandleImage.enabled}");
                        Debug.Log($"[ScrollViewCreator] Handle实际尺寸:{scrollbar.handleRect.rect.width}x{scrollbar.handleRect.rect.height}");
                        Debug.Log($"[ScrollViewCreator] Handle世界位置:{scrollbar.handleRect.position}");
                    }
                }

                Debug.Log("[ScrollViewCreator] ===== Scrollbar复制完成 =====");
                return scrollbarObj;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[ScrollViewCreator] 复制Scrollbar时发生错误: {ex.Message}");
                Debug.LogError($"[ScrollViewCreator] 堆栈: {ex.StackTrace}");
                return null;
            }
        }

        private static void SetLayerRecursively(GameObject obj, int layer)
        {
            obj.layer = layer;
            foreach (Transform child in obj.transform)
            {
                SetLayerRecursively(child.gameObject, layer);
            }
        }

        private static void ConfigureScrollRect(GameObject scrollViewObj, RectTransform contentRect,
            RectTransform viewportRect, GameObject? scrollbarObj)
        {
            Debug.Log("[ScrollViewCreator] 配置ScrollRect组件...");

            ScrollRect scrollRect = scrollViewObj.AddComponent<ScrollRect>();
            scrollRect.content = contentRect;
            scrollRect.viewport = viewportRect;
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
            scrollRect.movementType = ScrollRect.MovementType.Elastic;
            scrollRect.elasticity = 0.1f;
            scrollRect.inertia = true;
            scrollRect.decelerationRate = 0.135f;
            scrollRect.scrollSensitivity = 30f;

            if (scrollbarObj != null)
            {
                Scrollbar scrollbar = scrollbarObj.GetComponent<Scrollbar>();
                if (scrollbar != null)
                {
                    scrollRect.verticalScrollbar = scrollbar;
                    scrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.Permanent; // 改为永久显示，方便调试
                    scrollRect.verticalScrollbarSpacing = 0f;
                    Debug.Log("[ScrollViewCreator] ✓ 已关联Scrollbar到ScrollRect (Permanent模式)");
                }
            }

            Debug.Log("[ScrollViewCreator] ✓ ScrollRect配置完成");
        }

        private static void LogScrollViewInfo(RectTransform scrollViewRect, RectTransform viewportRect,
            RectTransform contentRect)
        {
            Debug.Log("[ScrollViewCreator] ===== ScrollView尺寸信息 =====");
            Debug.Log($"[ScrollViewCreator] ScrollView: {scrollViewRect.rect.width} x {scrollViewRect.rect.height}");
            Debug.Log($"[ScrollViewCreator] Viewport:   {viewportRect.rect.width} x {viewportRect.rect.height}");
            Debug.Log($"[ScrollViewCreator] Content:    {contentRect.rect.width} x {contentRect.rect.height}");
            Debug.Log($"[ScrollViewCreator] Scrollbar:  宽度={SCROLLBAR_WIDTH}");
        }

        public static void RemovePresetLayoutComponents(GameObject presetPanel)
        {
            if (presetPanel == null) return;

            try
            {
                Debug.Log("[ScrollViewCreator] 移除Preset面板的原有布局组件...");

                VerticalLayoutGroup? layoutGroup = presetPanel.GetComponent<VerticalLayoutGroup>();
                if (layoutGroup != null)
                {
                    UnityEngine.Object.DestroyImmediate(layoutGroup);
                    Debug.Log("[ScrollViewCreator] ✓ 已移除VerticalLayoutGroup");
                }

                ContentSizeFitter? sizeFitter = presetPanel.GetComponent<ContentSizeFitter>();
                if (sizeFitter != null)
                {
                    UnityEngine.Object.DestroyImmediate(sizeFitter);
                    Debug.Log("[ScrollViewCreator] ✓ 已移除ContentSizeFitter");
                }

                Debug.Log("[ScrollViewCreator] ✓ Preset面板原有布局组件清理完成");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[ScrollViewCreator] 移除布局组件时发生错误: {ex.Message}");
            }
        }
    }
}