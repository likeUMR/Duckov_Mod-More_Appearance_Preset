using System;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace MoreAppearancePreset
{
    /// <summary>
    /// UI调试导出工具 - 用于导出GameObject的完整UI信息
    /// </summary>
    public static class UIDebugExporter
    {
        /// <summary>
        /// 导出GameObject的完整UI信息到文件
        /// </summary>
        /// <param name="obj">要导出的GameObject</param>
        /// <param name="fileName">文件名（可选，默认使用对象名称）</param>
        public static void ExportToFile(GameObject obj, string? fileName = null)
        {
            if (obj == null)
            {
                Debug.LogError("[UIDebugExporter] 导出对象为null");
                return;
            }

            string outputFileName = fileName ?? $"{obj.name}_UIExport.txt";
            string outputPath = Path.Combine(Application.dataPath, outputFileName);

            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("=".PadRight(80, '='));
                sb.AppendLine($"UI Export Report - {obj.name}");
                sb.AppendLine($"Export Time: {DateTime.Now}");
                sb.AppendLine("=".PadRight(80, '='));
                sb.AppendLine();

                // 导出UI结构
                ExportUIInfoRecursive(obj, sb, 0);

                // 写入文件
                File.WriteAllText(outputPath, sb.ToString());
                Debug.Log($"[UIDebugExporter] ✓ UI信息已导出到: {outputPath}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[UIDebugExporter] 导出失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 导出GameObject的完整UI信息到日志
        /// </summary>
        public static void ExportToLog(GameObject obj)
        {
            if (obj == null)
            {
                Debug.LogError("[UIDebugExporter] 导出对象为null");
                return;
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("\n" + "=".PadRight(80, '='));
            sb.AppendLine($"UI Export Report - {obj.name}");
            sb.AppendLine("=".PadRight(80, '='));

            ExportUIInfoRecursive(obj, sb, 0);

            Debug.Log(sb.ToString());
        }

        /// <summary>
        /// 递归导出GameObject的UI信息
        /// </summary>
        private static void ExportUIInfoRecursive(GameObject obj, StringBuilder sb, int depth)
        {
            if (obj == null) return;

            string indent = new string(' ', depth * 4);
            string separator = "-".PadRight(70, '-');

            // GameObject基本信息
            sb.AppendLine($"{indent}{separator}");
            sb.AppendLine($"{indent}[GameObject] {obj.name}");
            sb.AppendLine($"{indent}  Active: {obj.activeSelf}");
            sb.AppendLine($"{indent}  Tag: {obj.tag}");
            sb.AppendLine($"{indent}  Layer: {LayerMask.LayerToName(obj.layer)}");

            // Transform信息
            Transform transform = obj.transform;
            sb.AppendLine($"{indent}  [Transform]");
            sb.AppendLine($"{indent}    localPosition: {transform.localPosition}");
            sb.AppendLine($"{indent}    localRotation: {transform.localRotation.eulerAngles}");
            sb.AppendLine($"{indent}    localScale: {transform.localScale}");

            // RectTransform信息（UI核心）
            RectTransform? rectTransform = obj.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                sb.AppendLine($"{indent}  [RectTransform] ★");
                sb.AppendLine($"{indent}    anchorMin: {rectTransform.anchorMin}");
                sb.AppendLine($"{indent}    anchorMax: {rectTransform.anchorMax}");
                sb.AppendLine($"{indent}    pivot: {rectTransform.pivot}");
                sb.AppendLine($"{indent}    sizeDelta: {rectTransform.sizeDelta}");
                sb.AppendLine($"{indent}    anchoredPosition: {rectTransform.anchoredPosition}");
                sb.AppendLine($"{indent}    anchoredPosition3D: {rectTransform.anchoredPosition3D}");
                sb.AppendLine($"{indent}    offsetMin: {rectTransform.offsetMin}");
                sb.AppendLine($"{indent}    offsetMax: {rectTransform.offsetMax}");
                sb.AppendLine($"{indent}    rect: {rectTransform.rect}");
            }

            // Canvas组件
            Canvas? canvas = obj.GetComponent<Canvas>();
            if (canvas != null)
            {
                sb.AppendLine($"{indent}  [Canvas]");
                sb.AppendLine($"{indent}    renderMode: {canvas.renderMode}");
                sb.AppendLine($"{indent}    sortingOrder: {canvas.sortingOrder}");
                sb.AppendLine($"{indent}    overrideSorting: {canvas.overrideSorting}");
            }

            // CanvasScaler组件
            CanvasScaler? canvasScaler = obj.GetComponent<CanvasScaler>();
            if (canvasScaler != null)
            {
                sb.AppendLine($"{indent}  [CanvasScaler]");
                sb.AppendLine($"{indent}    uiScaleMode: {canvasScaler.uiScaleMode}");
                sb.AppendLine($"{indent}    referenceResolution: {canvasScaler.referenceResolution}");
                sb.AppendLine($"{indent}    screenMatchMode: {canvasScaler.screenMatchMode}");
                sb.AppendLine($"{indent}    matchWidthOrHeight: {canvasScaler.matchWidthOrHeight}");
            }

            // Button组件
            Button? button = obj.GetComponent<Button>();
            if (button != null)
            {
                sb.AppendLine($"{indent}  [Button] ★");
                sb.AppendLine($"{indent}    interactable: {button.interactable}");
                sb.AppendLine($"{indent}    transition: {button.transition}");
                sb.AppendLine($"{indent}    navigation: {button.navigation.mode}");

                if (button.targetGraphic != null)
                {
                    sb.AppendLine($"{indent}    targetGraphic: {button.targetGraphic.GetType().Name} ({button.targetGraphic.gameObject.name})");
                }

                if (button.transition == Selectable.Transition.ColorTint)
                {
                    ColorBlock colors = button.colors;
                    sb.AppendLine($"{indent}    colors.normalColor: {colors.normalColor}");
                    sb.AppendLine($"{indent}    colors.highlightedColor: {colors.highlightedColor}");
                    sb.AppendLine($"{indent}    colors.pressedColor: {colors.pressedColor}");
                    sb.AppendLine($"{indent}    colors.selectedColor: {colors.selectedColor}");
                    sb.AppendLine($"{indent}    colors.disabledColor: {colors.disabledColor}");
                    sb.AppendLine($"{indent}    colors.colorMultiplier: {colors.colorMultiplier}");
                    sb.AppendLine($"{indent}    colors.fadeDuration: {colors.fadeDuration}");
                }

                sb.AppendLine($"{indent}    onClick.GetPersistentEventCount(): {button.onClick.GetPersistentEventCount()}");
            }

            // Image组件
            Image? image = obj.GetComponent<Image>();
            if (image != null)
            {
                sb.AppendLine($"{indent}  [Image]");
                sb.AppendLine($"{indent}    sprite: {image.sprite?.name ?? "null"}");
                sb.AppendLine($"{indent}    color: {image.color}");
                sb.AppendLine($"{indent}    raycastTarget: {image.raycastTarget}");
                sb.AppendLine($"{indent}    type: {image.type}");
                sb.AppendLine($"{indent}    fillCenter: {image.fillCenter}");
                sb.AppendLine($"{indent}    preserveAspect: {image.preserveAspect}");
            }

            // RawImage组件
            RawImage? rawImage = obj.GetComponent<RawImage>();
            if (rawImage != null)
            {
                sb.AppendLine($"{indent}  [RawImage]");
                sb.AppendLine($"{indent}    texture: {rawImage.texture?.name ?? "null"}");
                sb.AppendLine($"{indent}    color: {rawImage.color}");
                sb.AppendLine($"{indent}    raycastTarget: {rawImage.raycastTarget}");
            }

            // Text组件
            Text? text = obj.GetComponent<Text>();
            if (text != null)
            {
                sb.AppendLine($"{indent}  [Text]");
                sb.AppendLine($"{indent}    text: \"{text.text}\"");
                sb.AppendLine($"{indent}    font: {text.font?.name ?? "null"}");
                sb.AppendLine($"{indent}    fontSize: {text.fontSize}");
                sb.AppendLine($"{indent}    fontStyle: {text.fontStyle}");
                sb.AppendLine($"{indent}    color: {text.color}");
                sb.AppendLine($"{indent}    alignment: {text.alignment}");
                sb.AppendLine($"{indent}    alignByGeometry: {text.alignByGeometry}");
                sb.AppendLine($"{indent}    horizontalOverflow: {text.horizontalOverflow}");
                sb.AppendLine($"{indent}    verticalOverflow: {text.verticalOverflow}");
                sb.AppendLine($"{indent}    raycastTarget: {text.raycastTarget}");
                sb.AppendLine($"{indent}    resizeTextForBestFit: {text.resizeTextForBestFit}");
            }

            // ScrollRect组件
            ScrollRect? scrollRect = obj.GetComponent<ScrollRect>();
            if (scrollRect != null)
            {
                sb.AppendLine($"{indent}  [ScrollRect] ★★★");
                sb.AppendLine($"{indent}    content: {scrollRect.content?.name ?? "null"}");
                sb.AppendLine($"{indent}    viewport: {scrollRect.viewport?.name ?? "null"}");
                sb.AppendLine($"{indent}    horizontal: {scrollRect.horizontal}");
                sb.AppendLine($"{indent}    vertical: {scrollRect.vertical}");
                sb.AppendLine($"{indent}    movementType: {scrollRect.movementType}");
                sb.AppendLine($"{indent}    elasticity: {scrollRect.elasticity}");
                sb.AppendLine($"{indent}    inertia: {scrollRect.inertia}");
                sb.AppendLine($"{indent}    decelerationRate: {scrollRect.decelerationRate}");
                sb.AppendLine($"{indent}    scrollSensitivity: {scrollRect.scrollSensitivity}");

                if (scrollRect.horizontalScrollbar != null)
                {
                    sb.AppendLine($"{indent}    horizontalScrollbar: {scrollRect.horizontalScrollbar.name}");
                    sb.AppendLine($"{indent}    horizontalScrollbarVisibility: {scrollRect.horizontalScrollbarVisibility}");
                    sb.AppendLine($"{indent}    horizontalScrollbarSpacing: {scrollRect.horizontalScrollbarSpacing}");
                }

                if (scrollRect.verticalScrollbar != null)
                {
                    sb.AppendLine($"{indent}    verticalScrollbar: {scrollRect.verticalScrollbar.name}");
                    sb.AppendLine($"{indent}    verticalScrollbarVisibility: {scrollRect.verticalScrollbarVisibility}");
                    sb.AppendLine($"{indent}    verticalScrollbarSpacing: {scrollRect.verticalScrollbarSpacing}");
                }
            }

            // Scrollbar组件
            Scrollbar? scrollbar = obj.GetComponent<Scrollbar>();
            if (scrollbar != null)
            {
                sb.AppendLine($"{indent}  [Scrollbar]");
                sb.AppendLine($"{indent}    direction: {scrollbar.direction}");
                sb.AppendLine($"{indent}    value: {scrollbar.value}");
                sb.AppendLine($"{indent}    size: {scrollbar.size}");
                sb.AppendLine($"{indent}    numberOfSteps: {scrollbar.numberOfSteps}");
                sb.AppendLine($"{indent}    handleRect: {scrollbar.handleRect?.name ?? "null"}");
            }

            // Mask组件
            Mask? mask = obj.GetComponent<Mask>();
            if (mask != null)
            {
                sb.AppendLine($"{indent}  [Mask] ★");
                sb.AppendLine($"{indent}    showMaskGraphic: {mask.showMaskGraphic}");
            }

            // RectMask2D组件
            RectMask2D? rectMask2D = obj.GetComponent<RectMask2D>();
            if (rectMask2D != null)
            {
                sb.AppendLine($"{indent}  [RectMask2D]");
            }

            // LayoutGroup组件（父类）
            LayoutGroup? layoutGroup = obj.GetComponent<LayoutGroup>();
            if (layoutGroup != null)
            {
                string layoutType = layoutGroup.GetType().Name;
                sb.AppendLine($"{indent}  [{layoutType}] ★★");
                sb.AppendLine($"{indent}    padding: L={layoutGroup.padding.left} R={layoutGroup.padding.right} T={layoutGroup.padding.top} B={layoutGroup.padding.bottom}");
                sb.AppendLine($"{indent}    childAlignment: {layoutGroup.childAlignment}");

                // HorizontalOrVerticalLayoutGroup特有属性
                if (layoutGroup is HorizontalOrVerticalLayoutGroup hvLayoutGroup)
                {
                    sb.AppendLine($"{indent}    spacing: {hvLayoutGroup.spacing}");
                    sb.AppendLine($"{indent}    childForceExpandWidth: {hvLayoutGroup.childForceExpandWidth}");
                    sb.AppendLine($"{indent}    childForceExpandHeight: {hvLayoutGroup.childForceExpandHeight}");
                    sb.AppendLine($"{indent}    childControlWidth: {hvLayoutGroup.childControlWidth}");
                    sb.AppendLine($"{indent}    childControlHeight: {hvLayoutGroup.childControlHeight}");
                    sb.AppendLine($"{indent}    childScaleWidth: {hvLayoutGroup.childScaleWidth}");
                    sb.AppendLine($"{indent}    childScaleHeight: {hvLayoutGroup.childScaleHeight}");
                }

                // GridLayoutGroup特有属性
                if (layoutGroup is GridLayoutGroup gridLayoutGroup)
                {
                    sb.AppendLine($"{indent}    cellSize: {gridLayoutGroup.cellSize}");
                    sb.AppendLine($"{indent}    spacing: {gridLayoutGroup.spacing}");
                    sb.AppendLine($"{indent}    startCorner: {gridLayoutGroup.startCorner}");
                    sb.AppendLine($"{indent}    startAxis: {gridLayoutGroup.startAxis}");
                    sb.AppendLine($"{indent}    constraint: {gridLayoutGroup.constraint}");
                    sb.AppendLine($"{indent}    constraintCount: {gridLayoutGroup.constraintCount}");
                }
            }

            // ContentSizeFitter组件
            ContentSizeFitter? sizeFitter = obj.GetComponent<ContentSizeFitter>();
            if (sizeFitter != null)
            {
                sb.AppendLine($"{indent}  [ContentSizeFitter] ★");
                sb.AppendLine($"{indent}    horizontalFit: {sizeFitter.horizontalFit}");
                sb.AppendLine($"{indent}    verticalFit: {sizeFitter.verticalFit}");
            }

            // LayoutElement组件
            LayoutElement? layoutElement = obj.GetComponent<LayoutElement>();
            if (layoutElement != null)
            {
                sb.AppendLine($"{indent}  [LayoutElement]");
                sb.AppendLine($"{indent}    ignoreLayout: {layoutElement.ignoreLayout}");
                sb.AppendLine($"{indent}    minWidth: {layoutElement.minWidth}");
                sb.AppendLine($"{indent}    minHeight: {layoutElement.minHeight}");
                sb.AppendLine($"{indent}    preferredWidth: {layoutElement.preferredWidth}");
                sb.AppendLine($"{indent}    preferredHeight: {layoutElement.preferredHeight}");
                sb.AppendLine($"{indent}    flexibleWidth: {layoutElement.flexibleWidth}");
                sb.AppendLine($"{indent}    flexibleHeight: {layoutElement.flexibleHeight}");
            }

            // AspectRatioFitter组件
            AspectRatioFitter? aspectFitter = obj.GetComponent<AspectRatioFitter>();
            if (aspectFitter != null)
            {
                sb.AppendLine($"{indent}  [AspectRatioFitter]");
                sb.AppendLine($"{indent}    aspectMode: {aspectFitter.aspectMode}");
                sb.AppendLine($"{indent}    aspectRatio: {aspectFitter.aspectRatio}");
            }

            // 列出所有其他组件
            Component[] allComponents = obj.GetComponents<Component>();
            bool hasOtherComponents = false;
            StringBuilder otherComponentsSb = new StringBuilder();

            foreach (Component comp in allComponents)
            {
                if (comp == null) continue;

                Type compType = comp.GetType();
                // 跳过已经详细输出的组件
                if (compType == typeof(Transform) ||
                    compType == typeof(RectTransform) ||
                    compType == typeof(Canvas) ||
                    compType == typeof(CanvasScaler) ||
                    compType == typeof(Button) ||
                    compType == typeof(Image) ||
                    compType == typeof(RawImage) ||
                    compType == typeof(Text) ||
                    compType == typeof(ScrollRect) ||
                    compType == typeof(Scrollbar) ||
                    compType == typeof(Mask) ||
                    compType == typeof(RectMask2D) ||
                    typeof(LayoutGroup).IsAssignableFrom(compType) ||
                    compType == typeof(ContentSizeFitter) ||
                    compType == typeof(LayoutElement) ||
                    compType == typeof(AspectRatioFitter))
                {
                    continue;
                }

                if (!hasOtherComponents)
                {
                    hasOtherComponents = true;
                    otherComponentsSb.AppendLine($"{indent}  [Other Components]");
                }
                otherComponentsSb.AppendLine($"{indent}    - {compType.Name}");
            }

            if (hasOtherComponents)
            {
                sb.Append(otherComponentsSb.ToString());
            }

            // 子对象数量
            int childCount = obj.transform.childCount;
            if (childCount > 0)
            {
                sb.AppendLine($"{indent}  [Children Count]: {childCount}");
            }

            sb.AppendLine();

            // 递归导出所有子对象
            foreach (Transform child in obj.transform)
            {
                ExportUIInfoRecursive(child.gameObject, sb, depth + 1);
            }
        }

        /// <summary>
        /// 导出场景中的所有ScrollView
        /// </summary>
        public static void FindAndExportAllScrollViews()
        {
            Debug.Log("[UIDebugExporter] 开始搜索场景中的所有ScrollView...");

            ScrollRect[] allScrollRects = UnityEngine.Object.FindObjectsOfType<ScrollRect>(true);

            if (allScrollRects.Length == 0)
            {
                Debug.Log("[UIDebugExporter] 场景中未找到任何ScrollRect组件");
                return;
            }

            Debug.Log($"[UIDebugExporter] 找到 {allScrollRects.Length} 个ScrollRect组件:");

            foreach (ScrollRect scrollRect in allScrollRects)
            {
                string path = UIFinder.GetFullPath(scrollRect.transform);
                Debug.Log($"  - {path}");
            }

            // 导出第一个ScrollView作为参考
            if (allScrollRects.Length > 0)
            {
                Debug.Log("\n[UIDebugExporter] 导出第一个ScrollView作为参考:");
                ExportToLog(allScrollRects[0].gameObject);
            }
        }
    }
}