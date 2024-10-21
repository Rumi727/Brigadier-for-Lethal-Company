using Rumi.BrigadierForLethalCompany.API;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable CS1591 // 공개된 형식 또는 멤버에 대한 XML 주석이 없습니다.
namespace Rumi.BrigadierForLethalCompany.Components
{
    /// <summary>
    /// A script that displays an auto-complete list.
    /// <br/><br/>
    /// 자동 완성 목록을 표시하는 스크립트입니다
    /// </summary>
    public sealed class CommandIntelliSense : MonoBehaviour
    {
        public static TMP_FontAsset? font => HUDManager.Instance.chatText.font;

        public RectTransform rectTransform => _rectTransform ??= (RectTransform)transform;
        RectTransform? _rectTransform;

        public Image bgImage => _bgImage ??= GetComponent<Image>();
        Image? _bgImage;

        public VerticalLayoutGroup layout => _layout ??= GetComponent<VerticalLayoutGroup>();
        VerticalLayoutGroup? _layout;

        public ContentSizeFitter sizeFitter => _contentSizeFitter ??= GetComponent<ContentSizeFitter>();
        ContentSizeFitter? _contentSizeFitter;

        public HUDManager? hudManager { get; private set; }
        public TMP_InputField? chatField => hudManager != null ? hudManager.chatTextField : null;

        public Image? inputBgImage { get; private set; }
        public CommandIntelliSenseText? text { get; private set; }

        static readonly Color orgCaretColor = new Color(0.4157f, 0.3569f, 1, 0.7725f);
        static readonly Color orgTextColor = new Color(0.3451f, 0.3686f, 0.8196f, 0.8314f);

        int lastCaretPosition = 0;
        void Update()
        {
            if (chatField == null)
                return;

            if (lastCaretPosition != chatField.caretPosition)
            {
                UpdateIntelliSenseText(chatField.text);
                lastCaretPosition = chatField.caretPosition;
            }
        }

        /// <summary>
        /// Update intelliSense
        /// <br/><br/>
        /// 인탤리센스를 업데이트합니다
        /// </summary>
        /// <param name="input">
        /// The input value to be updated
        /// <br/><br/>
        /// 업데이트할 입력값입니다
        /// </param>
        public async void UpdateIntelliSenseText(string input)
        {
            if (text == null || inputBgImage == null || chatField == null)
                return;

            bool isCommand = input.StartsWith("/");
            if (isCommand && hudManager != null)
                text.text.text = await ServerCommand.dispatcher.GetIntelliSenseText(input.Remove(0, 1), new ServerCommandSource(hudManager.localPlayer), chatField.caretPosition - 1);
            else
                text.text.text = string.Empty;

            bgImage.color = string.IsNullOrEmpty(text.text.text) ? Color.clear : Color.black;
            inputBgImage.color = isCommand ? Color.black : Color.clear;

            chatField.caretColor = isCommand ? Color.white : orgCaretColor;
            chatField.textComponent.color = isCommand ? Color.white : orgTextColor;

            text.text.fontSize = 14 * Mathf.Clamp(30f / text.text.text.Count(static x => x == '\n'), 0, 1);

            UpdateSize();
        }

        /// <summary>
        /// Update the size of the text
        /// <br/><br/>
        /// 텍스트의 크기를 업데이트합니다
        /// </summary>
        public void UpdateSize()
        {
            if (text == null)
                return;

            text.sizeFitter.SetLayoutHorizontal();
            text.sizeFitter.SetLayoutVertical();
        }

        public static CommandIntelliSense Create(HUDManager hudManager)
        {
            TMP_InputField chatField = hudManager.chatTextField;
            CommandIntelliSense obj = new GameObject("Command Intelli Sense", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter)).AddComponent<CommandIntelliSense>();
            obj.transform.SetParent(chatField.transform, false);

            // Event
            chatField.onValueChanged.AddListener(obj.UpdateIntelliSenseText);

            // Rect Transform
            {
                obj.rectTransform.anchorMin = Vector2.up;
                obj.rectTransform.anchorMax = Vector2.up;

                obj.rectTransform.pivot = new Vector2(0, 0);
            }

            // Image
            obj.bgImage.color = Color.clear;

            // Vertical Layout Group
            {
                obj.layout.padding = new RectOffset(5, 5, 5, 5);

                obj.layout.childControlHeight = false;
                obj.layout.childForceExpandHeight = false;
            }

            // Content Size Fitter
            {
                obj.sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                obj.sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            }

            //Input Field BG
            {
                Image inputBg = new GameObject("Command Intelli Sense Input Field BG", typeof(RectTransform), typeof(CanvasRenderer)).AddComponent<Image>();

                inputBg.transform.SetParent(chatField.transform, false);
                inputBg.transform.SetAsFirstSibling();

                inputBg.rectTransform.anchorMin = Vector2.zero;
                inputBg.rectTransform.anchorMax = Vector2.one;

                inputBg.rectTransform.sizeDelta = Vector2.zero;

                inputBg.rectTransform.offsetMin = new Vector2(inputBg.rectTransform.offsetMin.x, inputBg.rectTransform.offsetMin.y + 37);
                inputBg.rectTransform.offsetMax = new Vector2(inputBg.rectTransform.offsetMax.x, inputBg.rectTransform.offsetMax.y - 2);

                inputBg.color = Color.clear;

                obj.inputBgImage = inputBg;
            }

            // Text
            {
                obj.hudManager = hudManager;
                obj.text = CommandIntelliSenseText.Create(obj);
            }

            // Layout Calc
            obj.UpdateSize();

            return obj;
        }
    }
}
