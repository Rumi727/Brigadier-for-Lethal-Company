using TMPro;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
namespace Rumi.BrigadierForLethalCompany.Components
{
    public sealed class CommandIntelliSenseText : MonoBehaviour
    {
        public RectTransform rectTransform => _rectTransform ??= (RectTransform)transform;
        RectTransform? _rectTransform;

        public TextMeshProUGUI text => _text ??= GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI? _text;

        public ContentSizeFitter sizeFitter => _sizeFitter ??= GetComponent<ContentSizeFitter>();
        ContentSizeFitter? _sizeFitter;

        public static CommandIntelliSenseText Create(CommandIntelliSense parent)
        {
            CommandIntelliSenseText obj = new GameObject("Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI), typeof(ContentSizeFitter)).AddComponent<CommandIntelliSenseText>();
            obj.transform.SetParent(parent.transform, false);

            obj.text.text = "";

            obj.text.font = CommandIntelliSense.font;
            obj.text.fontSize = 14;

            obj.text.enableWordWrapping = false;
            obj.text.alignment = TextAlignmentOptions.BottomLeft;

            // Content Size Fitter
            obj.sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            obj.sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            return obj;
        }
    }
}
