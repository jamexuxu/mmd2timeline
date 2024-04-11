/* /////////////////////////////////////////////////////////////////////////////////////////////////
Utils 2024-03-10

This is a feature extension to MacGruber_Utils

Licensed under CC BY-SA after EarlyAccess ended. (see https://creativecommons.org/licenses/by-sa/4.0/)

///////////////////////////////////////////////////////////////////////////////////////////////// */

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MacGruber
{
    public static partial class Utils
    {
        #region 参数名称与Label不一样的UI控件设定
        /// <summary>
        /// 设定参数名称与Label不一样的Toggle
        /// </summary>
        /// <param name="script"></param>
        /// <param name="paramName"></param>
        /// <param name="label"></param>
        /// <param name="defaultValue"></param>
        /// <param name="rightSide"></param>
        /// <returns></returns>
        public static JSONStorableBool SetupToggle(MVRScript script, string paramName, string label, bool defaultValue, bool rightSide)
        {
            JSONStorableBool storable = new JSONStorableBool(paramName, defaultValue);
            storable.storeType = JSONStorableParam.StoreType.Full;
            script.CreateToggle(storable, rightSide).label = label;
            script.RegisterBool(storable);
            return storable;
        }

        /// <summary>
        /// 设定参与名称与Label不一样的Slider
        /// </summary>
        /// <param name="script"></param>
        /// <param name="paramName"></param>
        /// <param name="label"></param>
        /// <param name="defaultValue"></param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <param name="rightSide"></param>
        /// <returns></returns>
        public static JSONStorableFloat SetupSliderFloat(MVRScript script, string paramName, string label, float defaultValue, float minValue, float maxValue, bool rightSide)
        {
            JSONStorableFloat storable = new JSONStorableFloat(paramName, defaultValue, minValue, maxValue, true, true);
            storable.storeType = JSONStorableParam.StoreType.Full;
            script.CreateSlider(storable, rightSide).label = label;
            script.RegisterFloat(storable);
            return storable;
        }

        /// <summary>
        /// 设定参数名称与Label不一样的Button
        /// </summary>
        /// <param name="script"></param>
        /// <param name="paramName"></param>
        /// <param name="label"></param>
        /// <param name="callback"></param>
        /// <param name="rightSide"></param>
        /// <returns></returns>
        public static UIDynamicButton SetupButton(MVRScript script, string paramName, string label, UnityAction callback, bool rightSide)
        {
            UIDynamicButton button = script.CreateButton(paramName, rightSide);
            button.label = label;
            button.button.onClick.AddListener(callback);
            return button;
        }

        /// <summary>
        /// 设定参数名称与Label不一样的Chooser
        /// </summary>
        /// <param name="self"></param>
        /// <param name="paramName"></param>
        /// <param name="label"></param>
        /// <param name="entries"></param>
        /// <param name="displayEntries"></param>
        /// <param name="defaultIndex"></param>
        /// <param name="rightSide"></param>
        /// <returns></returns>
        public static JSONStorableStringChooser SetupStringChooser(MVRScript self, string paramName, string label, List<string> entries, List<string> displayEntries, int defaultIndex, bool rightSide)
        {
            string defaultEntry = (defaultIndex >= 0 && defaultIndex < entries.Count) ? entries[defaultIndex] : "";
            JSONStorableStringChooser storable = new JSONStorableStringChooser(paramName, entries, displayEntries, defaultEntry, label);
            self.CreateScrollablePopup(storable, rightSide).label = label;
            self.RegisterStringChooser(storable);
            return storable;
        }

        /// <summary>
        /// 设定参数名称与Label不一样的SliderInt
        /// </summary>
        /// <param name="script"></param>
        /// <param name="paramName"></param>
        /// <param name="label"></param>
        /// <param name="defaultValue"></param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <param name="rightSide"></param>
        /// <returns></returns>
        public static JSONStorableFloat SetupSliderInt(MVRScript script, string paramName, string label, int defaultValue, int minValue, int maxValue, bool rightSide)
        {
            JSONStorableFloat storable = new JSONStorableFloat(paramName, defaultValue, minValue, maxValue, true, true);
            storable.storeType = JSONStorableParam.StoreType.Full;
            UIDynamicSlider slider = script.CreateSlider(storable, rightSide);
            slider.label = label;
            slider.slider.wholeNumbers = true;
            slider.valueFormat = "F0";
            script.RegisterFloat(storable);
            return storable;
        }

        /// <summary>
        /// 设定参数名与Label不一样的ColorPicker
        /// </summary>
        /// <param name="script"></param>
        /// <param name="paramName"></param>
        /// <param name="label"></param>
        /// <param name="color"></param>
        /// <param name="rightSide"></param>
        /// <returns></returns>
        public static JSONStorableColor SetupColor(MVRScript script, string paramName, string label, Color color, bool rightSide)
        {
            HSVColor hsvColor = HSVColorPicker.RGBToHSV(color.r, color.g, color.b);
            JSONStorableColor storable = new JSONStorableColor(paramName, hsvColor);
            storable.storeType = JSONStorableParam.StoreType.Full;
            script.CreateColorPicker(storable, rightSide).label = label;
            script.RegisterColor(storable);
            return storable;
        }

        /// <summary>
        /// 设定参数名与Label不一样的Float slider
        /// </summary>
        /// <param name="script"></param>
        /// <param name="label"></param>
        /// <param name="defaultValue"></param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <param name="rightSide"></param>
        /// <returns></returns>
        public static JSONStorableFloat SetupSliderFloatWithRange(MVRScript script, string paramName, string label, float defaultValue, float minValue, float maxValue, bool rightSide)
        {
            JSONStorableFloat storable = new JSONStorableFloat(paramName, defaultValue, minValue, maxValue, true, true);
            storable.storeType = JSONStorableParam.StoreType.Full;
            storable.constrained = false;
            UIDynamicSlider slider = script.CreateSlider(storable, rightSide);
            slider.label = label;
            slider.rangeAdjustEnabled = true;
            script.RegisterFloat(storable);
            return storable;
        }
        #endregion

        /// <summary>
        /// 创建带回调函数的Toggle
        /// </summary>
        /// <param name="script"></param>
        /// <param name="label"></param>
        /// <param name="defaultValue"></param>
        /// <param name="callback"></param>
        /// <param name="rightSide"></param>
        /// <returns></returns>
        public static JSONStorableBool SetupToggle(MVRScript script, string label, bool defaultValue, Action<bool> callback, bool rightSide)
        {
            JSONStorableBool storable = SetupToggle(script, label, defaultValue, rightSide);
            storable.setCallbackFunction = v => callback(v);
            return storable;
        }

        // Create VaM-UI Float slider
        public static JSONStorableFloat SetupSliderFloat(MVRScript script, string label, float defaultValue, float minValue, float maxValue, bool rightSide, string valueFormat = "")
        {
            JSONStorableFloat storable = new JSONStorableFloat(label, defaultValue, minValue, maxValue, true, true);
            storable.storeType = JSONStorableParam.StoreType.Full;
            var slider = script.CreateSlider(storable, rightSide);
            if (!string.IsNullOrEmpty(valueFormat))
            {
                slider.valueFormat = valueFormat;
            }
            //script.RegisterFloat(storable);
            return storable;
        }

        /// <summary>
        /// 创建带回调函数的Slider
        /// </summary>
        /// <param name="script"></param>
        /// <param name="label"></param>
        /// <param name="defaultValue"></param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <param name="callback"></param>
        /// <param name="rightSide"></param>
        /// <param name="valueFormat"></param>
        /// <returns></returns>
        public static JSONStorableFloat SetupSliderFloat(MVRScript script, string label, float defaultValue, float minValue, float maxValue, Action<float> callback, bool rightSide, string valueFormat = "")
        {
            JSONStorableFloat storable = SetupSliderFloat(script, label, defaultValue, minValue, maxValue, rightSide, valueFormat);
            storable.setCallbackFunction = v => callback(v);
            return storable;
        }

        /// <summary>
        /// 创建带回调函数的范围可变Slider
        /// </summary>
        /// <param name="script"></param>
        /// <param name="label"></param>
        /// <param name="defaultValue"></param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <param name="callback"></param>
        /// <param name="rightSide"></param>
        /// <param name="valueFormat"></param>
        /// <returns></returns>
        public static JSONStorableFloat SetupSliderFloatWithRange(MVRScript script, string label, float defaultValue, float minValue, float maxValue, Action<float> callback, bool rightSide, string valueFormat = "")
        {
            JSONStorableFloat storable = SetupSliderFloatWithRange(script, label, defaultValue, minValue, maxValue, rightSide, valueFormat);
            storable.setCallbackFunction = v => callback(v);
            return storable;
        }

        // Create VaM-UI Float slider
        public static JSONStorableFloat SetupSliderFloatWithRange(MVRScript script, string label, float defaultValue, float minValue, float maxValue, bool rightSide, string valueFormat = "")
        {
            JSONStorableFloat storable = new JSONStorableFloat(label, defaultValue, minValue, maxValue, true, true);
            storable.storeType = JSONStorableParam.StoreType.Full;
            storable.constrained = false;
            UIDynamicSlider slider = script.CreateSlider(storable, rightSide);
            slider.rangeAdjustEnabled = true;
            if (!string.IsNullOrEmpty(valueFormat))
            {
                slider.valueFormat = valueFormat;
            }
            //script.RegisterFloat(storable);
            return storable;
        }

        public static JSONStorableFloat SetupSliderIntWithRange(MVRScript script, string paramName, string label, int defaultValue, int minValue, int maxValue, bool rightSide)
        {
            JSONStorableFloat storable = new JSONStorableFloat(paramName, defaultValue, minValue, maxValue, true, true);
            storable.storeType = JSONStorableParam.StoreType.Full;
            storable.constrained = false;
            UIDynamicSlider slider = script.CreateSlider(storable, rightSide);
            slider.label = label;
            slider.label = label;
            slider.slider.wholeNumbers = true;
            slider.valueFormat = "F0";
            script.RegisterFloat(storable);
            return storable;
        }

        // Create VaM-UI TextureChooser. Note that you are responsible for destroying the texture when you don't need it anymore.
        public static JSONStorableUrl SetupTexture2DChooser(MVRScript self, string paramName, string label, string defaultValue, bool rightSide, TextureSettings settings, TextureSetCallback callback, bool infoText = true)
        {
            JSONStorableUrl storable = new JSONStorableUrl(paramName, string.Empty, (string url) => { QueueLoadTexture(url, settings, callback); }, "jpg|png|tif|tiff");
            self.RegisterUrl(storable);
            UIDynamicButton button = self.CreateButton(paramName, rightSide);
            button.label = label;
            if (infoText)
            {
                UIDynamicTextField textfield = self.CreateTextField(storable, rightSide);
                textfield.UItext.alignment = TextAnchor.MiddleRight;
                textfield.UItext.horizontalOverflow = HorizontalWrapMode.Overflow;
                textfield.UItext.verticalOverflow = VerticalWrapMode.Truncate;
                LayoutElement layout = textfield.GetComponent<LayoutElement>();
                layout.preferredHeight = layout.minHeight = 35;
                textfield.height = 35;
            }
            if (!string.IsNullOrEmpty(defaultValue))
                storable.SetFilePath(defaultValue);
            storable.RegisterFileBrowseButton(button.button);
            return storable;
        }

        // ===========================================================================================
        // Custom UI system with new UI elements and the ability to easily add/remove UI at runtime
        //
        // Usage instructions:
        // - Before using the custom UI elements, call from your MVRScript:
        //       Utils.OnInitUI(CreateUIElement);
        // - When your MVRScript receives the OnDestroy message call:
        //       Utils.OnDestroyUI();

        // Create one-line text input with label
        public static UIDynamicLabelInput SetupTextInput(MVRScript script, string label, JSONStorableString storable, bool? rightSide)
        {
            if (ourLabelWithInputPrefab == null)
            {
                ourLabelWithInputPrefab = new GameObject("LabelInput");
                ourLabelWithInputPrefab.SetActive(false);
                RectTransform rt = ourLabelWithInputPrefab.AddComponent<RectTransform>();
                rt.anchorMax = new Vector2(0, 1);
                rt.anchorMin = new Vector2(0, 1);
                rt.offsetMax = new Vector2(535, -500);
                rt.offsetMin = new Vector2(10, -600);
                LayoutElement le = ourLabelWithInputPrefab.AddComponent<LayoutElement>();
                le.flexibleWidth = 1;
                le.minHeight = 40;
                le.minWidth = 350;
                le.preferredHeight = 40;
                le.preferredWidth = 500;

                RectTransform backgroundTransform = script.manager.configurableScrollablePopupPrefab.transform.Find("Background") as RectTransform;
                backgroundTransform = UnityEngine.Object.Instantiate(backgroundTransform, ourLabelWithInputPrefab.transform);
                backgroundTransform.name = "Background";
                backgroundTransform.anchorMax = new Vector2(1, 1);
                backgroundTransform.anchorMin = new Vector2(0, 0);
                backgroundTransform.offsetMax = new Vector2(0, 0);
                backgroundTransform.offsetMin = new Vector2(0, -10);

                RectTransform labelTransform = script.manager.configurableScrollablePopupPrefab.transform.Find("Button/Text") as RectTransform; ;
                labelTransform = UnityEngine.Object.Instantiate(labelTransform, ourLabelWithInputPrefab.transform);
                labelTransform.name = "Text";
                labelTransform.anchorMax = new Vector2(0, 1);
                labelTransform.anchorMin = new Vector2(0, 0);
                labelTransform.offsetMax = new Vector2(155, -5);
                labelTransform.offsetMin = new Vector2(5, 0);
                Text labelText = labelTransform.GetComponent<Text>();
                labelText.text = "Name";
                labelText.color = Color.white;

                RectTransform inputTransform = script.manager.configurableTextFieldPrefab.transform as RectTransform;
                inputTransform = UnityEngine.Object.Instantiate(inputTransform, ourLabelWithInputPrefab.transform);
                inputTransform.anchorMax = new Vector2(1, 1);
                inputTransform.anchorMin = new Vector2(0, 0);
                inputTransform.offsetMax = new Vector2(-5, -5);
                inputTransform.offsetMin = new Vector2(160, -5);
                UIDynamicTextField textfield = inputTransform.GetComponent<UIDynamicTextField>();
                textfield.backgroundColor = Color.white;
                LayoutElement layout = textfield.GetComponent<LayoutElement>();
                layout.preferredHeight = layout.minHeight = 35;
                InputField inputfield = textfield.gameObject.AddComponent<InputField>();
                inputfield.textComponent = textfield.UItext;

                RectTransform textTransform = textfield.UItext.rectTransform;
                textTransform.anchorMax = new Vector2(1, 1);
                textTransform.anchorMin = new Vector2(0, 0);
                textTransform.offsetMax = new Vector2(-5, -5);
                textTransform.offsetMin = new Vector2(10, -5);

                UnityEngine.Object.Destroy(textfield);

                UIDynamicLabelInput uid = ourLabelWithInputPrefab.AddComponent<UIDynamicLabelInput>();
                uid.label = labelText;
                uid.input = inputfield;
            }

            {
                var createUIElement = GetCreateUIElement(script);

                Transform t = createUIElement(ourLabelWithInputPrefab.transform, rightSide);
                UIDynamicLabelInput uid = t.gameObject.GetComponent<UIDynamicLabelInput>();
                storable.inputField = uid.input;
                uid.label.text = label;
                t.gameObject.SetActive(true);
                return uid;
            }
        }

        // Create label that as an X button on the right side.
        public static UIDynamicLabelXButton SetupLabelXButton(MVRScript script, string label, UnityAction callback, bool? rightSide)
        {
            if (ourLabelWithXButtonPrefab == null)
            {
                ourLabelWithXButtonPrefab = new GameObject("LabelXButton");
                ourLabelWithXButtonPrefab.SetActive(false);
                RectTransform rt = ourLabelWithXButtonPrefab.AddComponent<RectTransform>();
                rt.anchorMax = new Vector2(0, 1);
                rt.anchorMin = new Vector2(0, 1);
                rt.offsetMax = new Vector2(535, -500);
                rt.offsetMin = new Vector2(10, -600);
                LayoutElement le = ourLabelWithXButtonPrefab.AddComponent<LayoutElement>();
                le.flexibleWidth = 1;
                le.minHeight = 50;
                le.minWidth = 350;
                le.preferredHeight = 50;
                le.preferredWidth = 500;

                RectTransform backgroundTransform = script.manager.configurableScrollablePopupPrefab.transform.Find("Background") as RectTransform;
                backgroundTransform = UnityEngine.Object.Instantiate(backgroundTransform, ourLabelWithXButtonPrefab.transform);
                backgroundTransform.name = "Background";
                backgroundTransform.anchorMax = new Vector2(1, 1);
                backgroundTransform.anchorMin = new Vector2(0, 0);
                backgroundTransform.offsetMax = new Vector2(0, 0);
                backgroundTransform.offsetMin = new Vector2(0, -10);

                RectTransform buttonTransform = script.manager.configurableScrollablePopupPrefab.transform.Find("Button") as RectTransform;
                buttonTransform = UnityEngine.Object.Instantiate(buttonTransform, ourLabelWithXButtonPrefab.transform);
                buttonTransform.name = "Button";
                buttonTransform.anchorMax = new Vector2(1, 1);
                buttonTransform.anchorMin = new Vector2(1, 0);
                buttonTransform.offsetMax = new Vector2(0, 0);
                buttonTransform.offsetMin = new Vector2(-60, -10);
                Button buttonButton = buttonTransform.GetComponent<Button>();
                Text buttonText = buttonTransform.Find("Text").GetComponent<Text>();
                buttonText.text = "X";

                RectTransform labelTransform = buttonText.rectTransform;
                labelTransform = UnityEngine.Object.Instantiate(labelTransform, ourLabelWithXButtonPrefab.transform);
                labelTransform.name = "Text";
                labelTransform.anchorMax = new Vector2(1, 1);
                labelTransform.anchorMin = new Vector2(0, 0);
                labelTransform.offsetMax = new Vector2(-65, 0);
                labelTransform.offsetMin = new Vector2(5, -10);
                Text labelText = labelTransform.GetComponent<Text>();
                labelText.verticalOverflow = VerticalWrapMode.Overflow;

                UIDynamicLabelXButton uid = ourLabelWithXButtonPrefab.AddComponent<UIDynamicLabelXButton>();
                uid.label = labelText;
                uid.button = buttonButton;
            }

            {
                var createUIElement = GetCreateUIElement(script);

                Transform t = createUIElement(ourLabelWithXButtonPrefab.transform, rightSide);
                UIDynamicLabelXButton uid = t.gameObject.GetComponent<UIDynamicLabelXButton>();
                uid.label.text = label;
                uid.button.onClick.AddListener(callback);
                t.gameObject.SetActive(true);
                return uid;
            }
        }

        public static UIDynamicTextInfo SetupInfoTextNoScroll(MVRScript script, string text, float height, bool? rightSide)
        {
            if (ourTextInfoPrefab == null)
            {
                ourTextInfoPrefab = new GameObject("TextInfo");
                ourTextInfoPrefab.SetActive(false);
                RectTransform rt = ourTextInfoPrefab.AddComponent<RectTransform>();
                rt.anchorMax = new Vector2(0, 1);
                rt.anchorMin = new Vector2(0, 1);
                rt.offsetMax = new Vector2(535, -500);
                rt.offsetMin = new Vector2(10, -600);
                LayoutElement le = ourTextInfoPrefab.AddComponent<LayoutElement>();
                le.flexibleWidth = 1;
                le.minHeight = 35;
                le.minWidth = 350;
                le.preferredHeight = 35;
                le.preferredWidth = 500;

                RectTransform backgroundTransform = script.manager.configurableScrollablePopupPrefab.transform.Find("Background") as RectTransform;
                backgroundTransform = UnityEngine.Object.Instantiate(backgroundTransform, ourTextInfoPrefab.transform);
                backgroundTransform.name = "Background";
                backgroundTransform.anchorMax = new Vector2(1, 1);
                backgroundTransform.anchorMin = new Vector2(0, 0);
                backgroundTransform.offsetMax = new Vector2(0, 0);
                backgroundTransform.offsetMin = new Vector2(0, -10);

                RectTransform labelTransform = script.manager.configurableScrollablePopupPrefab.transform.Find("Button/Text") as RectTransform; ;
                labelTransform = UnityEngine.Object.Instantiate(labelTransform, ourTextInfoPrefab.transform);
                labelTransform.name = "Text";
                labelTransform.anchorMax = new Vector2(1, 1);
                labelTransform.anchorMin = new Vector2(0, 0);
                labelTransform.offsetMax = new Vector2(-5, 0);
                labelTransform.offsetMin = new Vector2(5, 0);
                Text labelText = labelTransform.GetComponent<Text>();
                labelText.alignment = TextAnchor.UpperLeft;

                UIDynamicTextInfo uid = ourTextInfoPrefab.AddComponent<UIDynamicTextInfo>();
                uid.text = labelText;
                uid.layout = le;
                uid.background = backgroundTransform;
            }

            {
                var createUIElement = GetCreateUIElement(script);

                Transform t = createUIElement(ourTextInfoPrefab.transform, rightSide);
                UIDynamicTextInfo uid = t.gameObject.GetComponent<UIDynamicTextInfo>();
                uid.text.text = text;

                uid.height = height;
                //uid.layout.minHeight = height;
                //uid.layout.preferredHeight = height;
                t.gameObject.SetActive(true);
                return uid;
            }
        }

        public static UIDynamicTextInfo SetupInfoTextNoScroll(MVRScript script, JSONStorableString storable, float height, bool? rightSide)
        {
            UIDynamicTextInfo uid = SetupInfoTextNoScroll(script, storable.val, height, rightSide);
            storable.setCallbackFunction = (string text) =>
            {
                if (uid != null && uid.text != null)
                    uid.text.text = text;
            };
            return uid;
        }

        public static UIDynamicTextInfo SetupInfoOneLine(MVRScript script, string text, bool? rightSide)
        {
            UIDynamicTextInfo uid = SetupInfoTextNoScroll(script, text, 35, rightSide);
            uid.background.offsetMin = new Vector2(0, 0);
            return uid;
        }

        public static UIDynamicTextInfo SetupInfoOneLine(MVRScript script, JSONStorableString storable, bool? rightSide)
        {
            UIDynamicTextInfo uid = SetupInfoTextNoScroll(script, storable, 35, rightSide);
            uid.background.offsetMin = new Vector2(0, 0);
            return uid;
        }

        public static UIDynamicTwinButton SetupTwinButton(MVRScript script, string leftLabel, UnityAction leftCallback, string rightLabel, UnityAction rightCallback, bool? rightSide)
        {
            if (ourTwinButtonPrefab == null)
            {
                ourTwinButtonPrefab = new GameObject("TwinButton");
                ourTwinButtonPrefab.SetActive(false);
                RectTransform rt = ourTwinButtonPrefab.AddComponent<RectTransform>();
                rt.anchorMax = new Vector2(0, 1);
                rt.anchorMin = new Vector2(0, 1);
                rt.offsetMax = new Vector2(535, -500);
                rt.offsetMin = new Vector2(10, -600);
                LayoutElement le = ourTwinButtonPrefab.AddComponent<LayoutElement>();
                le.flexibleWidth = 1;
                le.minHeight = 50;
                le.minWidth = 350;
                le.preferredHeight = 50;
                le.preferredWidth = 500;

                RectTransform buttonTransform = script.manager.configurableScrollablePopupPrefab.transform.Find("Button") as RectTransform;
                buttonTransform = UnityEngine.Object.Instantiate(buttonTransform, ourTwinButtonPrefab.transform);
                buttonTransform.name = "ButtonLeft";
                buttonTransform.anchorMax = new Vector2(0.5f, 1.0f);
                buttonTransform.anchorMin = new Vector2(0.0f, 0.0f);
                buttonTransform.offsetMax = new Vector2(-3, 0);
                buttonTransform.offsetMin = new Vector2(0, 0);
                Button buttonLeft = buttonTransform.GetComponent<Button>();
                Text labelLeft = buttonTransform.Find("Text").GetComponent<Text>();

                buttonTransform = UnityEngine.Object.Instantiate(buttonTransform, ourTwinButtonPrefab.transform);
                buttonTransform.name = "ButtonRight";
                buttonTransform.anchorMax = new Vector2(1.0f, 1.0f);
                buttonTransform.anchorMin = new Vector2(0.5f, 0.0f);
                buttonTransform.offsetMax = new Vector2(0, 0);
                buttonTransform.offsetMin = new Vector2(3, 0);
                Button buttonRight = buttonTransform.GetComponent<Button>();
                Text labelRight = buttonTransform.Find("Text").GetComponent<Text>();

                UIDynamicTwinButton uid = ourTwinButtonPrefab.AddComponent<UIDynamicTwinButton>();
                uid.labelLeft = labelLeft;
                uid.labelRight = labelRight;
                uid.buttonLeft = buttonLeft;
                uid.buttonRight = buttonRight;
            }

            {
                var createUIElement = GetCreateUIElement(script);

                Transform t = createUIElement(ourTwinButtonPrefab.transform, rightSide);
                UIDynamicTwinButton uid = t.GetComponent<UIDynamicTwinButton>();
                uid.labelLeft.text = leftLabel;
                uid.labelRight.text = rightLabel;
                uid.buttonLeft.onClick.AddListener(leftCallback);
                uid.buttonRight.onClick.AddListener(rightCallback);
                t.gameObject.SetActive(true);
                return uid;
            }
        }

        public static UIDynamicTwinToggle SetupTwinToggle(MVRScript script, string leftLabel, UnityAction leftCallback, string rightLabel, UnityAction rightCallback, bool? rightSide)
        {
            if (ourTwinTogglePrefab == null)
            {
                ourTwinTogglePrefab = new GameObject("TwinToggle");
                ourTwinTogglePrefab.SetActive(false);
                RectTransform rt = ourTwinTogglePrefab.AddComponent<RectTransform>();
                rt.anchorMax = new Vector2(0, 1);
                rt.anchorMin = new Vector2(0, 1);
                rt.offsetMax = new Vector2(535, -500);
                rt.offsetMin = new Vector2(10, -600);
                LayoutElement le = ourTwinTogglePrefab.AddComponent<LayoutElement>();
                le.flexibleWidth = 1;
                le.minHeight = 50;
                le.minWidth = 350;
                le.preferredHeight = 50;
                le.preferredWidth = 500;

                Transform togglePrefab = script.manager.configurableTogglePrefab.transform;
                // foreach(Transform child in togglePrefab) child.name.Print();
                RectTransform labelTransform = togglePrefab.Find("Label") as RectTransform;
                // foreach(Transform child in labelTransform) child.name.Print();
                // labelTransform.NullCheck();
                labelTransform = UnityEngine.Object.Instantiate(labelTransform, ourTwinTogglePrefab.transform);
                labelTransform.name = "LabelLeft1";
                labelTransform.anchorMax = new Vector2(0.5f, 1.0f);
                labelTransform.anchorMin = new Vector2(0.0f, 0.0f);
                labelTransform.offsetMax = new Vector2(-3, 0);
                labelTransform.offsetMin = new Vector2(0, 0);
                // foreach(var comp in labelTransform.GetComponents(typeof(Component))) comp.GetType().Print();
                // labelTransform.NullCheck();
                // Toggle toggleLeft = labelTransform.GetComponent<Toggle>();labelTransform.NullCheck();
                Text labelLeft = labelTransform.GetComponent<Text>();

                RectTransform panelTransform = togglePrefab.Find("Panel") as RectTransform;
                // foreach(Transform child in labelTransform) child.name.Print();
                // labelTransform.NullCheck();
                panelTransform = UnityEngine.Object.Instantiate(panelTransform, ourTwinTogglePrefab.transform);
                panelTransform.name = "PanelLeft";
                panelTransform.anchorMax = new Vector2(.1f, 1f);
                // panelTransform.anchorMin = new Vector2(0.0f, 0.0f);
                // panelTransform.offsetMax = new Vector2(-3, 0);
                // panelTransform.offsetMin = new Vector2(0, 0);
                // foreach(var comp in labelTransform.GetComponents(typeof(Component))) comp.GetType().Print();
                // labelTransform.NullCheck();
                // Toggle toggleLeft = labelTransform.GetComponent<Toggle>();labelTransform.NullCheck();
                // Text panelLeft = panelTransform.GetComponent<Text>();

                RectTransform backGroundTransform = togglePrefab.Find("Panel") as RectTransform;
                // foreach(Transform child in labelTransform) child.name.Print();
                // labelTransform.NullCheck();
                backGroundTransform = UnityEngine.Object.Instantiate(backGroundTransform, ourTwinTogglePrefab.transform);
                backGroundTransform.name = "BackGroundLeft";
                backGroundTransform.anchorMax = new Vector2(.5f, 1f);
                // panelTransform.anchorMin = new Vector2(0.0f, 0.0f);
                // panelTransform.offsetMax = new Vector2(-3, 0);
                // panelTransform.offsetMin = new Vector2(0, 0);
                foreach (var comp in backGroundTransform.GetComponents(typeof(Component))) comp.GetType();
                // labelTransform.NullCheck();
                // Toggle toggleLeft = labelTransform.GetComponent<Toggle>();labelTransform.NullCheck();
                // Text panelLeft = panelTransform.GetComponent<Text>();




                labelTransform = UnityEngine.Object.Instantiate(labelTransform, ourTwinTogglePrefab.transform);
                labelTransform.name = "LabelRight1";
                labelTransform.anchorMax = new Vector2(1.0f, 1.0f);
                labelTransform.anchorMin = new Vector2(0.5f, 0.0f);
                labelTransform.offsetMax = new Vector2(0, 0);
                labelTransform.offsetMin = new Vector2(3, 0);
                // foreach(var comp in labelTransform.GetComponents(typeof(Component))) comp.GetType().Print();
                // labelTransform.NullCheck();
                // Toggle toggleLeft = labelTransform.GetComponent<Toggle>();labelTransform.NullCheck();

                Text labelRight = labelTransform.GetComponent<Text>();


                // labelTransform = UnityEngine.Object.Instantiate(labelTransform, ourTwinTogglePrefab.transform);
                // labelTransform.name = "ButtonRight";
                // labelTransform.anchorMax = new Vector2(1.0f, 1.0f);
                // labelTransform.anchorMin = new Vector2(0.5f, 0.0f);
                // labelTransform.offsetMax = new Vector2(0, 0);
                // labelTransform.offsetMin = new Vector2(3, 0);
                // Toggle toggleRight = labelTransform.GetComponent<Toggle>();
                // Text labelRight = labelTransform.Find("Text").GetComponent<Text>();

                UIDynamicTwinToggle uid = ourTwinTogglePrefab.AddComponent<UIDynamicTwinToggle>();
                uid.labelLeft = labelLeft;
                uid.labelLeft = labelLeft;
                uid.labelRight = labelRight;
                // uid.toggleLeft = toggleLeft;
                // uid.toggleRight = toggleRight;
            }

            {
                var createUIElement = GetCreateUIElement(script);

                Transform t = createUIElement(ourTwinTogglePrefab.transform, rightSide);
                UIDynamicTwinToggle uid = t.GetComponent<UIDynamicTwinToggle>();
                uid.labelLeft.text = leftLabel;
                uid.labelRight.text = rightLabel;
                // uid.toggleLeft.onClick.AddListener(leftCallback);
                // uid.toggleRight.onClick.AddListener(rightCallback);
                t.gameObject.SetActive(true);
                return uid;
            }
        }

        public delegate Transform CreateUIElement(Transform prefab, bool? rightSide);
        public static void OnInitUI(CreateUIElement createUIElementCallback, MVRScript script = null)
        {
            if (script != null)
            {
                ourCreateUIElements[script] = createUIElementCallback;
            }
            else
            {
                ourCreateUIElement = createUIElementCallback;
            }
        }

        public static void OnDestroyUI(MVRScript script)
        {
            if (script != null)
            {
                if (ourCreateUIElements.ContainsKey(script))
                {
                    ourCreateUIElements.Remove(script);
                }
            }

            OnDestroyUI();
        }

        private static CreateUIElement GetCreateUIElement(MVRScript script)
        {
            if (script != null)
            {
                if (ourCreateUIElements.ContainsKey(script))
                {
                    return ourCreateUIElements[script];
                }
            }

            if (ourCreateUIElement != null)
            {
                return ourCreateUIElement;
            }

            throw new Exception("Using the custom UI elements, call from your MVRScript: Utils.OnInitUI(this,CreateUIElement);");
        }

        private static Dictionary<MVRScript, CreateUIElement> ourCreateUIElements = new Dictionary<MVRScript, CreateUIElement>();
        private static GameObject ourTwinTogglePrefab;
    }

    public class UIDynamicTwinToggle : UIDynamicUtils
    {
        public Text labelLeft;
        public Text labelRight;
        public Toggle toggleLeft;
        public Toggle toggleRight;
    }
}
