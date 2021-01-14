using AdaptiveRoads.Util;
using ColossalFramework.UI;
using KianCommons;
using KianCommons.UI;
using System;
using UnityEngine;

namespace AdaptiveRoads.UI.RoadEditor.MenuStyle {
    public class LoadTemplatePanel : PersitancyPanelBase {
        public SummaryLabel SummaryBox;
        public SavesListBoxProp SavesListBox;
        public UIButton LoadButton;
        public MenuCheckbox ToggleDir;
        public MenuCheckbox ToggleSide;
        public MenuTextFieldInt Displacement;

        public delegate void OnPropsLoadedHandler(NetLaneProps.Prop[] props);
        public event OnPropsLoadedHandler OnPropsLoaded;

        public static LoadTemplatePanel Display(OnPropsLoadedHandler handler) {
            Log.Debug($"LoadTemplatePanel.Display() called");
            var ret = UIView.GetAView().AddUIComponent<LoadTemplatePanel>();
            ret.OnPropsLoaded = handler;
            return ret;
        }

        public override void Awake() {
            base.Awake();
            AddDrag("Load Prop Template");
            {
                UIPanel panel = AddLeftPanel();
                {
                    SavesListBox = panel.AddUIComponent<SavesListBoxProp>();
                    SavesListBox.width = panel.width;
                    SavesListBox.height = 628;
                    SavesListBox.AddScrollBar();
                    SavesListBox.eventSelectedIndexChanged += (_, val) =>
                        OnSelectedSaveChanged(val);
                    SavesListBox.eventDoubleClick += (_, __) => OnLoad();
                }

            }
            {
                UIPanel panel = AddRightPanel();
                {
                    SummaryBox = panel.AddUIComponent<SummaryLabel>();
                    SummaryBox.width = panel.width;
                    SummaryBox.height = 400;
                }
                {
                    ToggleDir = panel.AddUIComponent<MenuCheckbox>();
                    ToggleDir.Label = "Toggle Forward/Backward";
                    ToggleSide = panel.AddUIComponent<MenuCheckbox>();
                    ToggleSide.Label = "Toggle RHT/LHT";
                }
                {
                    //Displacement = panel.AddUIComponent<TextFieldInt>();
                    //Displacement.width = panel.width;

                    UIPanel panel2 = panel.AddUIComponent<UIPanel>();
                    panel2.autoLayout = true;
                    panel2.autoLayoutDirection = LayoutDirection.Horizontal;
                    panel2.autoLayoutPadding = new RectOffset(0, 5, 0, 0);
                    var lbl = panel2.AddUIComponent<UILabel>();
                    lbl.text = "Displacement:";
                    Displacement = panel2.AddUIComponent<MenuTextFieldInt>();
                    Displacement.width = panel.width - Displacement.relativePosition.x;
                    Displacement.tooltip = "put a posetive number to move props sideways.";
                    lbl.height = Displacement.height;
                    lbl.verticalAlignment = UIVerticalAlignment.Middle;
                    panel2.FitChildren();
                }

            }

            FitChildrenVertically(10);

            {
                var BottomPanel = AddBottomPanel(this);
                LoadButton = BottomPanel.AddUIComponent<MenuButton>();
                LoadButton.text = "Load";
                LoadButton.eventClick += (_, __) => OnLoad();
                //pos.x += -LoadButton.size.x - 20;
                //LoadButton.relativePosition = pos;

                var cancel = BottomPanel.AddUIComponent<MenuButton>();
                cancel.text = "Cancel";
                cancel.eventClick += (_, __) => Destroy(gameObject);
                //var pos = size - cancel.size - new Vector2(20, 10);
                //cancel.relativePosition = pos;
            }
        }


        bool started_ = false;
        public override void Start() {
            Log.Debug("LoadTemplatePanel.Start() called");
            base.Start();
            started_ = true;
        }

        public override void OnDestroy() {
            this.SetAllDeclaredFieldsToNull();
            base.OnDestroy();
        }

        public void OnLoad() {
            var template = SavesListBox.SelectedTemplate;
            var props = template.GetProps();
            foreach (var prop in props) {
                if (ToggleDir.isChecked)
                    prop.ToggleForwardBackward();
                if (ToggleSide.isChecked)
                    prop.ToggleRHT_LHT();
                if (Displacement.Number != 0) {
                    prop.Displace(Displacement.Number);
                }
            }
            OnPropsLoaded(props);
            Destroy(gameObject);
        }

        public void OnSelectedSaveChanged(int newIndex) {
            Log.Debug($"OnSelectedSaveChanged({newIndex})\n" + Environment.StackTrace);
            try {
                if (started_) {
                    LoadButton.isEnabled = newIndex >= 0;
                    SummaryBox.text = SavesListBox.SelectedTemplate?.Summary ?? "";
                }
            } catch (Exception ex) {
                Log.Exception(ex, $"newIndex={newIndex} " +
                    $"SelectedIndex={SavesListBox.selectedIndex} " +
                    $"SelectedTemplate={SavesListBox.SelectedTemplate.ToSTR()} " +
                    $"Saves[0]={SavesListBox.Saves[0].ToSTR()}");
            }
        }

    }
}