using UnityEngine;
using UnityEngine.UI;

namespace MoreAppearancePreset
{
    /// <summary>
    /// 彩虹色循环效果组件
    /// </summary>
    public class RainbowColorEffect : MonoBehaviour
    {
        private Image? _image;
        private float _hue = 0f; // HSV中的H值，范围0-1
        
        // 参考颜色的RGB值: (0.91, 0.57, 0.08)
        private static readonly Color _referenceColor = new Color(0.91f, 0.57f, 0.08f, 1f);
        private float _saturation; // S值，从参考颜色提取
        private float _value; // V值，从参考颜色提取
        private const float _speed = 0.5f; // 颜色变化速度（每秒循环次数）

        void Start()
        {
            // 获取Image组件
            _image = GetComponent<Image>();
            if (_image == null)
            {
                Debug.LogWarning("[RainbowColorEffect] 未找到Image组件，将禁用此效果");
                enabled = false;
                return;
            }

            // 从参考颜色提取饱和度和亮度
            Color.RGBToHSV(_referenceColor, out float refH, out _saturation, out _value);
            
            // 保存原有的alpha值
            float originalAlpha = _image.color.a;
            
            // 初始化颜色（使用参考颜色的S和V，H从0开始）
            Color initialColor = Color.HSVToRGB(_hue, _saturation, _value);
            initialColor.a = originalAlpha;
            _image.color = initialColor;
            
            Debug.Log($"[RainbowColorEffect] 初始化完成 - Saturation: {_saturation:F3}, Value: {_value:F3}");
        }

        void Update()
        {
            if (_image == null)
            {
                return;
            }

            // 更新H值，实现循环
            _hue += _speed * Time.deltaTime;
            if (_hue >= 1f)
            {
                _hue -= 1f; // 循环回到0
            }

            // 将HSV转换为RGB（使用固定的S和V值）
            Color color = Color.HSVToRGB(_hue, _saturation, _value);
            
            // 保持原有的alpha值
            color.a = _image.color.a;
            
            // 应用颜色
            _image.color = color;
        }
    }
}

