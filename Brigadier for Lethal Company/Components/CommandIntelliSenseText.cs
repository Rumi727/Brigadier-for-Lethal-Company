using Rumi.BrigadierForLethalCompany.API;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
namespace Rumi.BrigadierForLethalCompany.Components
{
    public sealed class CommandIntelliSenseText : MonoBehaviour
    {
        public RectTransform rectTransform => field ??= (RectTransform)transform;

        StringBuilder builder = new();

        public NetworkIntelliSenseArray intelliSenseArray
        {
            get;
            set
            {
                field = value;

                if (field.type == NetworkIntelliSenseArray.Type.usage)
                {
                    textComponent.text = $"<color=#aaaaaa>{string.Join('\n', field.Select(x => x.text))}</color>";
                    _selectedLine = 0;
                }
                else if (field.type == NetworkIntelliSenseArray.Type.exception)
                {
                    textComponent.text = $"<color=#ff5555>{string.Join('\n', field.Select(x => x.text))}</color>";
                    _selectedLine = 0;
                }
                else
                {
                    _selectedLine = Mathf.Clamp(_selectedLine, 0, field.length - 1);

                    builder.Clear();

                    int i = selectedLine - 5;
                    int length = selectedLine + 6;

                    if (-i > 0)
                    {
                        length -= i;
                        i = 0;
                    }
                    if (length > field.length)
                    {
                        if (i >= length - field.length)
                            i -= length - field.length;
                        else
                            i = 0;

                        length = field.length;
                    }

                    for (; i < length; i++)
                    {
                        if (i == selectedLine)
                            builder.AppendLine($"<color=yellow>{field[i].text}</color>");
                        else
                            builder.AppendLine(field[i].text);
                    }

                    textComponent.text = builder.ToString();
                }

                sizeFitter.SetLayoutHorizontal();
                sizeFitter.SetLayoutVertical();
            }
        }

        public int selectedLine
        {
            get => _selectedLine;
            set
            {
                if (intelliSenseArray.type != NetworkIntelliSenseArray.Type.suggestion)
                {
                    _selectedLine = 0;
                    return;
                }

                if (value < 0)
                    _selectedLine = intelliSenseArray.length - 1;
                else if (value >= intelliSenseArray.length)
                    _selectedLine = 0;
                else
                    _selectedLine = value;

                intelliSenseArray = intelliSenseArray;
            }
        }
        int _selectedLine = 0;

        public TextMeshProUGUI textComponent => field ??= GetComponent<TextMeshProUGUI>();

        public ContentSizeFitter sizeFitter => field ??= GetComponent<ContentSizeFitter>();

        public static CommandIntelliSenseText Create(CommandIntelliSense parent)
        {
            CommandIntelliSenseText obj = new GameObject("Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI), typeof(ContentSizeFitter)).AddComponent<CommandIntelliSenseText>();
            obj.transform.SetParent(parent.transform, false);

            obj.textComponent.text = "";

            obj.textComponent.font = CommandIntelliSense.font;
            obj.textComponent.fontSize = 14;

            obj.textComponent.enableWordWrapping = false;
            obj.textComponent.alignment = TextAlignmentOptions.BottomLeft;

            // Content Size Fitter
            obj.sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            obj.sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            return obj;
        }
    }
}
