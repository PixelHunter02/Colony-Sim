// using UnityEditor;
// using UnityEditor.UIElements;
// using UnityEngine.UIElements;
//
// public class SliderWithInput : VisualElement
// {
//     public new class UxmlFactory: UxmlFactory<SliderWithInput>{}
//
//     public SliderWithInput()
//     {
//         SliderWithInput slider = this.Q<SliderWithInput>("Volume");
//         slider.Init();
//     }
//     
//     private Slider Slider => this.Q<Slider>("slider");
//     private FloatField Input => this.Q<FloatField>("input");
//
//     public SliderWithInput(SerializedProperty Property, string Label = "", float MinValue = 0, float MaxValue = 100)
//     {
//         Init(Property, Label, MinValue, MaxValue);
//     }
//
//     private void Init(SerializedProperty Property, string Label, float MinValue, float MaxValue)
//     {
//         VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath <VisualTreeAsset>("Assets/UI/Slider.uxml");
//         asset.CloneTree(this);
//
//         Slider.lowValue = MinValue;
//         Slider.highValue = MaxValue;
//         Slider.label = Label;
//         Slider.BindProperty(Property);
//         Input.BindProperty(Property);
//     }
// }
