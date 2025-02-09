﻿#region File Description
//-----------------------------------------------------------------------------
// This program show GeonBit.UI examples and usage.
//
// GeonBit.UI is an export of the UI system used for GeonBit (an open source 
// game engine in MonoGame) and is free to use under the MIT license.
//
// To learn more about GeonBit.UI, you can visit the git repo:
// https://github.com/RonenNess/GeonBit.UI
//
// Or explore the different README files scattered in the solution directory. 
//
// Author: Ronen Ness.
// Since: 2016.
//-----------------------------------------------------------------------------
#endregion

// using MonoGame and basic system stuff
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

// using GeonBit UI elements
using GeonBit.UI.Entities;
using GeonBit.UI.Entities.TextValidators;
using GeonBit.UI.DataTypes;
using GeonBit.UI.Utils.Forms;

namespace GeonBit.UI.Examples
{
    /// <summary>
    /// GeonBit.UI.Example is just an example code. Everything here is not a part of the GeonBit.UI framework, but merely an example of how to use it.
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGenerated]
    class NamespaceDoc
    {
    }

    /// <summary>
    /// This is the main 'Game' instance for your game.
    /// </summary>
    public class GeonBitUI_Examples : Game
    {
        // graphics and spritebatch
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // all the example panels (screens)
        List<Panel> panels = new List<Panel>();

        // buttons to rotate examples
        Button nextExampleButton;
        Button previousExampleButton;

        // paragraph that shows the currently active entity
        Paragraph targetEntityShow;

        // current example shown
        int currExample = 0;

        // current theme
        BuiltinThemes _currTheme;

        /// <summary>
        /// Create the game instance.
        /// </summary>
        public GeonBitUI_Examples()
        {
            // init graphics device manager and set content root
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Window.IsBorderless = true;
        }

        /// <summary>
        /// Initialize the main application.
        /// </summary>
        protected override void Initialize()
        {
            // make the window fullscreen (but still with border and top control bar)
            int _ScreenWidth = graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Width;
            int _ScreenHeight = graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Height;
            graphics.PreferredBackBufferWidth = (int)_ScreenWidth;
            graphics.PreferredBackBufferHeight = (int)_ScreenHeight;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();

            // init theme and ui
            InitializeThemeAndUI(BuiltinThemes.hd);
        }

        private void InitializeThemeAndUI(BuiltinThemes theme)
        {
            // clear previous panels
            panels.Clear();

            // store current theme
            _currTheme = theme;

            // create and init the UI manager
            UserInterface.Initialize(Content, theme);
            UserInterface.Active.UseRenderTarget = true;

            // draw cursor outside the render target
            UserInterface.Active.IncludeCursorInRenderTarget = false;

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = spriteBatch ?? new SpriteBatch(GraphicsDevice);

            // init ui and examples
            InitExamplesAndUI();
        }
        
        /// <summary>
        /// Create the top bar with next / prev buttons etc, and init all UI example panels.
        /// </summary>        
        protected void InitExamplesAndUI()
        {
            // will init examples only if true
            bool initExamples = true;

            // create top panel
            int topPanelHeight = 65;
            Panel topPanel = new Panel(new Vector2(0, topPanelHeight + 2), PanelSkin.Simple, Anchor.TopCenter);
            topPanel.Padding = Vector2.Zero;
            UserInterface.Active.AddEntity(topPanel);

            // add previous example button
            previousExampleButton = new Button("<- Back", ButtonSkin.Default, Anchor.Auto, new Vector2(300, topPanelHeight));
            previousExampleButton.OnClick = (Entity btn) => { this.PreviousExample(); };
            topPanel.AddChild(previousExampleButton);

            // add next example button
            nextExampleButton = new Button("Next ->", ButtonSkin.Default, Anchor.TopRight, new Vector2(300, topPanelHeight));
            nextExampleButton.OnClick = (Entity btn) => { this.NextExample(); };
            nextExampleButton.Identifier = "next_btn";
            topPanel.AddChild(nextExampleButton);

            // add show-get button
            Button showGitButton = new Button("Git Repo", ButtonSkin.Fancy, Anchor.TopCenter, new Vector2(280, topPanelHeight));
            showGitButton.OnClick = (Entity btn) => 
            {
                try
                {
                    System.Diagnostics.Process.Start("https://github.com/RonenNess/GeonBit.UI");
                }
                catch
                {
                    System.Diagnostics.Process.Start("IExplore", "https://github.com/RonenNess/GeonBit.UI");
                }
            };
            topPanel.AddChild(showGitButton);

            // add exit button
            Button exitBtn = new Button("Exit", anchor: Anchor.BottomRight, size: new Vector2(200, -1));
            exitBtn.OnClick = (Entity entity) => { Exit(); };
            UserInterface.Active.AddEntity(exitBtn);

            // events panel for debug
            Panel eventsPanel = new Panel(new Vector2(400, 530), PanelSkin.Simple, Anchor.CenterLeft, new Vector2(-10, 0));
            eventsPanel.Visible = false;

            // events log (single-time events)
            eventsPanel.AddChild(new Label("Events Log:"));
            SelectList eventsLog = new SelectList(size: new Vector2(-1, 280));
            eventsLog.ExtraSpaceBetweenLines = -8;
            eventsLog.ItemsScale = 0.5f;
            eventsLog.Locked = true;
            eventsPanel.AddChild(eventsLog);

            // current events (events that happen while something is true)
            eventsPanel.AddChild(new Label("Current Events:"));
            SelectList eventsNow = new SelectList(size: new Vector2(-1, 100));
            eventsNow.ExtraSpaceBetweenLines = -8;
            eventsNow.ItemsScale = 0.5f;
            eventsNow.Locked = true;
            eventsPanel.AddChild(eventsNow);

            // paragraph to show currently active panel
            targetEntityShow = new Paragraph("test", Anchor.Auto, Color.White, scale: 0.75f);
            eventsPanel.AddChild(targetEntityShow);

            // add the events panel
            UserInterface.Active.AddEntity(eventsPanel);

            // whenever events log list size changes, make sure its not too long. if it is, trim it.
            eventsLog.OnListChange = (Entity entity) =>
            {
                SelectList list = (SelectList)entity;
                if (list.Count > 100)
                {
                    list.RemoveItem(0);
                }
            };

            // listen to all global events - one timers
            UserInterface.Active.OnClick = (Entity entity) => { eventsLog.AddItem("Click: " + entity.GetType().Name); eventsLog.scrollToEnd();};
            UserInterface.Active.OnRightClick = (Entity entity) => { eventsLog.AddItem("RightClick: " + entity.GetType().Name); eventsLog.scrollToEnd(); };
            UserInterface.Active.OnMouseDown = (Entity entity) => { eventsLog.AddItem("MouseDown: " + entity.GetType().Name); eventsLog.scrollToEnd(); };
            UserInterface.Active.OnRightMouseDown = (Entity entity) => { eventsLog.AddItem("RightMouseDown: " + entity.GetType().Name); eventsLog.scrollToEnd(); };
            UserInterface.Active.OnMouseEnter = (Entity entity) => { eventsLog.AddItem("MouseEnter: " + entity.GetType().Name); eventsLog.scrollToEnd(); };
            UserInterface.Active.OnMouseLeave = (Entity entity) => { eventsLog.AddItem("MouseLeave: " + entity.GetType().Name); eventsLog.scrollToEnd(); };
            UserInterface.Active.OnMouseReleased = (Entity entity) => { eventsLog.AddItem("MouseReleased: " + entity.GetType().Name); eventsLog.scrollToEnd(); };
            UserInterface.Active.OnMouseWheelScroll = (Entity entity) => { eventsLog.AddItem("Scroll: " + entity.GetType().Name); eventsLog.scrollToEnd(); };
            UserInterface.Active.OnStartDrag = (Entity entity) => { eventsLog.AddItem("StartDrag: " + entity.GetType().Name); eventsLog.scrollToEnd(); };
            UserInterface.Active.OnStopDrag = (Entity entity) => { eventsLog.AddItem("StopDrag: " + entity.GetType().Name); eventsLog.scrollToEnd(); };
            UserInterface.Active.OnFocusChange = (Entity entity) => { eventsLog.AddItem("FocusChange: " + entity.GetType().Name); eventsLog.scrollToEnd(); };
            UserInterface.Active.OnValueChange = (Entity entity) => { if (entity.Parent == eventsLog) { return; } eventsLog.AddItem("ValueChanged: " + entity.GetType().Name); eventsLog.scrollToEnd(); };

            // clear the current events after every frame they were drawn
            eventsNow.AfterDraw = (Entity entity) => { eventsNow.ClearItems(); };

            // listen to all global events - happening now
            UserInterface.Active.WhileDragging = (Entity entity) => { eventsNow.AddItem("Dragging: " + entity.GetType().Name); eventsNow.scrollToEnd(); };
            UserInterface.Active.WhileMouseDown = (Entity entity) => { eventsNow.AddItem("MouseDown: " + entity.GetType().Name); eventsNow.scrollToEnd(); };
            UserInterface.Active.WhileMouseHover = (Entity entity) => { eventsNow.AddItem("MouseHover: " + entity.GetType().Name); eventsNow.scrollToEnd(); };
            eventsNow.MaxItems = 4;

            // add extra info button
            var offsetX = 140;
            Button infoBtn = new Button("  Events", anchor: Anchor.BottomLeft, size: new Vector2(280, -1), offset: new Vector2(offsetX, 0));
            offsetX += 280;
            infoBtn.AddChild(new Icon(IconType.Scroll, Anchor.CenterLeft), true);
            infoBtn.OnClick = (Entity entity) =>
            {
                eventsPanel.Visible = !eventsPanel.Visible;
            };
            infoBtn.ToggleMode = true;
            infoBtn.ToolTipText = "Show events log.";
            UserInterface.Active.AddEntity(infoBtn);

            // add button to apply transformations
            Button transBtn = new Button("Transform UI", anchor: Anchor.BottomLeft, size: new Vector2(290, -1), offset: new Vector2(offsetX, 0));
            offsetX += 290;
            transBtn.OnClick = (Entity entity) =>
            {
                if (UserInterface.Active.RenderTargetTransformMatrix == null)
                {
                    UserInterface.Active.RenderTargetTransformMatrix = Matrix.CreateScale(0.6f) * 
                        Matrix.CreateRotationZ(0.05f) * 
                        Matrix.CreateTranslation(new Vector3(150, 150, 0));
                }
                else
                {
                    UserInterface.Active.RenderTargetTransformMatrix = null;
                }
            };
            transBtn.ToggleMode = true;
            transBtn.ToolTipText = "Apply transform matrix on the entire UI.";
            UserInterface.Active.AddEntity(transBtn);

            // add button to enable debug mode
            Button debugBtn = new Button("Debug Mode", anchor: Anchor.BottomLeft, size: new Vector2(260, -1), offset: new Vector2(offsetX, 0));
            offsetX += 260;
            debugBtn.OnClick = (Entity entity) =>
            {
                UserInterface.Active.DebugDraw = !UserInterface.Active.DebugDraw;
            };
            debugBtn.ToggleMode = true;
            debugBtn.ToolTipText = "Enable special debug drawing mode.";
            UserInterface.Active.AddEntity(debugBtn);

            // add button to rptate theme debug mode
            Button changeThemeBtn = new Button("Change Theme", anchor: Anchor.BottomLeft, size: new Vector2(260, -1), offset: new Vector2(offsetX, 0));
            offsetX += 260;
            changeThemeBtn.OnClick = (Entity entity) =>
            {
                int theme = (int)_currTheme + 1;
                if (theme > (int)BuiltinThemes.editor)
                {
                    theme = 0;
                }
                InitializeThemeAndUI((BuiltinThemes)theme);
            };
            changeThemeBtn.ToolTipText = "Rotate through the built-in themes.";
            UserInterface.Active.AddEntity(changeThemeBtn);

            // zoom in / out factor
            float zoominFactor = 0.05f;

            // scale show
            Paragraph scaleShow = new Paragraph("100%", Anchor.BottomLeft, offset: new Vector2(10, 70));
            UserInterface.Active.AddEntity(scaleShow);

            // init zoom-out button
            Button zoomout = new Button(string.Empty, ButtonSkin.Default, Anchor.BottomLeft, new Vector2(70, 70));
            Icon zoomoutIcon = new Icon(IconType.ZoomOut, Anchor.Center, 0.75f);
            zoomout.AddChild(zoomoutIcon, true);
            zoomout.OnClick = (Entity btn) => {
                if (UserInterface.Active.GlobalScale > 0.5f)
                    UserInterface.Active.GlobalScale -= zoominFactor;
                scaleShow.Text = ((int)System.Math.Round(UserInterface.Active.GlobalScale * 100f)).ToString() + "%";
            };
            UserInterface.Active.AddEntity(zoomout);

            // init zoom-in button
            Button zoomin = new Button(string.Empty, ButtonSkin.Default, Anchor.BottomLeft, new Vector2(70, 70), new Vector2(70, 0));
            Icon zoominIcon = new Icon(IconType.ZoomIn, Anchor.Center, 0.75f);
            zoomin.AddChild(zoominIcon, true);
            zoomin.OnClick = (Entity btn) => {
                if (UserInterface.Active.GlobalScale < 1.45f)
                    UserInterface.Active.GlobalScale += zoominFactor;
                scaleShow.Text = ((int)System.Math.Round(UserInterface.Active.GlobalScale * 100f)).ToString() + "%";
            };
            UserInterface.Active.AddEntity(zoomin);

            // init all examples

            if (initExamples)
            {

                // example: welcome message
                {
                    // create panel and add to list of panels and manager
                    Panel panel = new Panel(new Vector2(520, -1));
                    panel.Draggable = true;
                    panels.Add(panel);
                    UserInterface.Active.AddEntity(panel);

                    // add title and text
                    Image title = new Image(Content.Load<Texture2D>("example/GeonBitUI-sm"), new Vector2(400, 240), anchor: Anchor.TopCenter, offset: new Vector2(0, -20));
                    title.ShadowColor = new Color(0, 0, 0, 128);
                    title.ShadowOffset = Vector2.One * -6;
                    panel.AddChild(title);
                    var welcomeText = new RichParagraph(@"Welcome to {{RED}}GeonBit{{MAGENTA}}.UI{{DEFAULT}}!

GeonBit.UI is the UI system of the GeonBit project.
It provide a simple yet extensive UI for MonoGame based projects.

To start the demo, please click the {{ITALIC}}'Next'{{DEFAULT}} button on the top bar.

");
                    panel.AddChild(welcomeText);
                    panel.AddChild(new Paragraph("V" + UserInterface.VERSION, Anchor.BottomRight)).FillColor = Color.Yellow;
                }

                // example: features list
                {
                    // create panel and add to list of panels and manager
                    Panel panel = new Panel(new Vector2(500, -1));
                    panels.Add(panel);
                    UserInterface.Active.AddEntity(panel);

                    // add title and text
                    panel.AddChild(new Header("Widgets Types"));
                    panel.AddChild(new HorizontalLine());
                    panel.AddChild(new Paragraph(@"GeonBit.UI implements the following widgets:

- Paragraphs
- Headers
- Buttons
- Panels
- CheckBox
- Radio buttons
- Rectangles
- Images & Icons
- Select List
- Dropdown
- Panel Tabs
- Sliders & Progressbars
- Text input
- Tooltip Text
- And more...
"));
                }

                // example: basic concepts
                {
                    // create panel and add to list of panels and manager
                    Panel panel = new Panel(new Vector2(740, -1));
                    panels.Add(panel);
                    UserInterface.Active.AddEntity(panel);

                    // add title and text
                    panel.AddChild(new Header("Basic Concepts"));
                    panel.AddChild(new HorizontalLine());
                    panel.AddChild(new Paragraph(@"Panels are the basic containers of GeonBit.UI. They are like window forms.

To position elements inside panels or other widgets, you set an anchor and offset. An anchor is a pre-defined position in parent element, like top-left corner, center, etc. and offset is just the distance from that point.

Another thing to keep in mind is size; Most widgets come with a default size, but for those you need to set size for remember that setting size 0 will take full width / height. For example, size of X = 0, Y = 100 means the widget will be 100 pixels height and the width of its parent (minus the parent padding)."));
                }

                // example: anchors
                {
                    // create panel and add to list of panels and manager
                    Panel panel = new Panel(new Vector2(800, 620));
                    panels.Add(panel);
                    UserInterface.Active.AddEntity(panel);

                    // add title and text
                    panel.AddChild(new Paragraph(@"Anchors help position elements. For example, this paragraph anchor is 'center'.

The most common anchors are 'Auto' and 'AutoInline', which will place entities one after another automatically.",
                        Anchor.Center, Color.White, 0.8f, new Vector2(320, 0)));

                    panel.AddChild(new Header("Anchors", Anchor.TopCenter, new Vector2(0, 100)));
                    panel.AddChild(new Paragraph("top-left", Anchor.TopLeft, Color.Yellow, 0.8f));
                    panel.AddChild(new Paragraph("top-center", Anchor.TopCenter, Color.Yellow, 0.8f));
                    panel.AddChild(new Paragraph("top-right", Anchor.TopRight, Color.Yellow, 0.8f));
                    panel.AddChild(new Paragraph("bottom-left", Anchor.BottomLeft, Color.Yellow, 0.8f));
                    panel.AddChild(new Paragraph("bottom-center", Anchor.BottomCenter, Color.Yellow, 0.8f));
                    panel.AddChild(new Paragraph("bottom-right", Anchor.BottomRight, Color.Yellow, 0.8f));
                    panel.AddChild(new Paragraph("center-left", Anchor.CenterLeft, Color.Yellow, 0.8f));
                    panel.AddChild(new Paragraph("center-right", Anchor.CenterRight, Color.Yellow, 0.8f));
                }

                // example: buttons
                {
                    // create panel and add to list of panels and manager
                    Panel panel = new Panel(new Vector2(450, -1));
                    panels.Add(panel);
                    UserInterface.Active.AddEntity(panel);

                    // add title and text
                    panel.AddChild(new Header("Buttons"));
                    panel.AddChild(new HorizontalLine());
                    panel.AddChild(new Paragraph("GeonBit.UI comes with 3 button skins:"));

                    // add default buttons
                    panel.AddChild(new Button("Default", ButtonSkin.Default));
                    panel.AddChild(new Button("Alternative", ButtonSkin.Alternative));
                    panel.AddChild(new Button("Fancy", ButtonSkin.Fancy));

                    // custom button
                    Button custom = new Button("Custom Skin", ButtonSkin.Default, size: new Vector2(0, 80));
                    custom.SetCustomSkin(
                        Content.Load<Texture2D>("example/btn_default"),
                        Content.Load<Texture2D>("example/btn_hover"),
                        Content.Load<Texture2D>("example/btn_down"));
                    panel.AddChild(custom);

                    // toggle button
                    panel.AddChild(new LineSpace());
                    panel.AddChild(new HorizontalLine());
                    panel.AddChild(new LineSpace());
                    panel.AddChild(new Paragraph("Note: buttons can also work in toggle mode:"));
                    Button btn = new Button("Toggle Me!", ButtonSkin.Default);
                    btn.ToggleMode = true;
                    panel.AddChild(btn);
                }

                // example: checkboxes and radio buttons
                {
                    // create panel and add to list of panels and manager
                    Panel panel = new Panel(new Vector2(450, -1));
                    panels.Add(panel);
                    UserInterface.Active.AddEntity(panel);

                    // checkboxes example
                    panel.AddChild(new Header("CheckBox"));
                    panel.AddChild(new HorizontalLine());
                    panel.AddChild(new Paragraph("CheckBoxes example:"));

                    panel.AddChild(new CheckBox("CheckBox 1"));
                    panel.AddChild(new CheckBox("CheckBox 2"));

                    // radio example
                    panel.AddChild(new LineSpace(3));
                    panel.AddChild(new Header("Radio buttons"));
                    panel.AddChild(new HorizontalLine());
                    panel.AddChild(new Paragraph("Radio buttons example:"));

                    panel.AddChild(new RadioButton("Option 1"));
                    panel.AddChild(new RadioButton("Option 2"));
                    panel.AddChild(new RadioButton("Option 3"));
                }

                // example: panels
                {
                    // create panel and add to list of panels and manager
                    Panel panel = new Panel(new Vector2(450, -1));
                    panels.Add(panel);
                    UserInterface.Active.AddEntity(panel);

                    // title and text
                    panel.AddChild(new Header("Panels"));
                    panel.AddChild(new HorizontalLine());
                    panel.AddChild(new Paragraph("GeonBit.UI comes with 4 alternative panel skins:"));
                    int panelHeight = 80;
                    {
                        Panel intPanel = new Panel(new Vector2(0, panelHeight), PanelSkin.Fancy, Anchor.Auto);
                        intPanel.AddChild(new Paragraph("Fancy Panel", Anchor.Center));
                        panel.AddChild(intPanel);
                    }
                    {
                        Panel intPanel = new Panel(new Vector2(0, panelHeight), PanelSkin.Alternative, Anchor.Auto);
                        intPanel.AddChild(new Paragraph("Alternative Panel", Anchor.Center));
                        panel.AddChild(intPanel);
                    }
                    {
                        Panel intPanel = new Panel(new Vector2(0, panelHeight), PanelSkin.Simple, Anchor.Auto);
                        intPanel.AddChild(new Paragraph("Simple Panel", Anchor.Center));
                        panel.AddChild(intPanel);
                    }
                    {
                        Panel intPanel = new Panel(new Vector2(0, panelHeight), PanelSkin.ListBackground, Anchor.Auto);
                        intPanel.AddChild(new Paragraph("List Background", Anchor.Center));
                        panel.AddChild(intPanel);
                    }
                    {
                        Panel intPanel = new Panel(new Vector2(0, panelHeight * 2), PanelSkin.ListBackground, Anchor.Auto);
                        intPanel.SetCustomSkin(Content.Load<Texture2D>("example/btn_default"));
                        intPanel.AddChild(new Paragraph("Custom Skin", Anchor.Center));
                        panel.AddChild(intPanel);
                    }
                }

                // example: draggable
                {
                    // create panel and add to list of panels and manager
                    Panel panel = new Panel(new Vector2(450, 690));
                    panel.Draggable = true;
                    panels.Add(panel);
                    UserInterface.Active.AddEntity(panel);

                    // title and text
                    panel.AddChild(new Header("Draggable"));
                    panel.AddChild(new HorizontalLine());
                    panel.AddChild(new Paragraph("This panel can be dragged, try it out!"));
                    panel.AddChild(new LineSpace());
                    panel.AddChild(new HorizontalLine());
                    panel.AddChild(new LineSpace());
                    Paragraph paragraph = new Paragraph("Any type of entity can be dragged. For example, try to drag this text!");
                    paragraph.SetStyleProperty("FillColor", new StyleProperty(Color.Yellow));
                    paragraph.SetStyleProperty("FillColor", new StyleProperty(Color.Purple), EntityState.MouseHover);
                    paragraph.Draggable = true;
                    paragraph.LimitDraggingToParentBoundaries = false;
                    panel.AddChild(paragraph);

                    // internal panel with internal draggable
                    Panel panelInt = new Panel(new Vector2(250, 250), PanelSkin.Alternative, Anchor.AutoCenter);
                    panelInt.Draggable = true;
                    panelInt.AddChild(new Paragraph("This panel is draggable too, but limited to its parent boundaries.", Anchor.Center, Color.White, 0.85f));
                    panel.AddChild(panelInt);
                }

                // example: animators
                {
                    // create panel and add to list of panels and manager
                    Panel panel = new Panel(new Vector2(550, -1));
                    panels.Add(panel);
                    UserInterface.Active.AddEntity(panel);

                    // add title and text
                    panel.AddChild(new Header("Animators"));
                    panel.AddChild(new HorizontalLine());
                    panel.AddChild(new Paragraph(@"Animators are classes that bring UI elements into life by animating them. For example, take a look at these animators:"));

                    // float up-down
                    {
                        panel.AddChild(new LineSpace(2));
                        var entity = panel.AddChild(new Paragraph(@"Float Up-Down animator"));
                        entity.AttachAnimator(new Animators.FloatUpDownAnimator());
                    }

                    // wave animation
                    {
                        panel.AddChild(new LineSpace(2));
                        var entity = panel.AddChild(new RichParagraph(@"Wave text animator"));
                        entity.AttachAnimator(new Animators.TextWaveAnimator());
                    }

                    // fade out
                    {
                        panel.AddChild(new LineSpace(2));
                        var entity = panel.AddChild(new Button(@"Fade Out (click to see)"));
                        var animator = entity.AttachAnimator(new Animators.FadeOutAnimator() { Enabled = false });
                        entity.OnClick += (Entity ent) =>
                        {
                            animator.Enabled = true;
                        };
                    }

                    // type writer animator
                    {
                        panel.AddChild(new LineSpace(2));
                        var entity = panel.AddChild(new RichParagraph(@""));
                        var animator = entity.AttachAnimator(new Animators.TypeWriterAnimator()
                        {
                            TextToType = @"This is a type writer animation, text will appear as if someone is typing it in real time. {{YELLOW}}Click on the paragraph to reset animation."
                        });
                        entity.OnClick += (Entity ent) =>
                        {
                            animator.Reset();
                        };
                    }
                }

                // example: sliders
                {
                    // create panel and add to list of panels and manager
                    Panel panel = new Panel(new Vector2(450, -1));
                    panels.Add(panel);
                    UserInterface.Active.AddEntity(panel);

                    // sliders title
                    panel.AddChild(new Header("Sliders"));
                    panel.AddChild(new HorizontalLine());
                    panel.AddChild(new Paragraph("Sliders help pick numeric value in range:"));

                    panel.AddChild(new Paragraph("\nDefault slider"));
                    {
                        var slider = panel.AddChild(new Slider(-10, 10, SliderSkin.Default)) as Slider;
                        var valueLabel = new Label("Value: 0");
                        slider.OnValueChange = (Entity entity) =>
                        {
                            valueLabel.Text = "Value: " + slider.Value;
                        };
                        panel.AddChild(valueLabel);
                    }

                    panel.AddChild(new Paragraph("\nFancy slider"));
                    panel.AddChild(new Slider(0, 10, SliderSkin.Fancy));

                    // progressbar title
                    panel.AddChild(new LineSpace(3));
                    panel.AddChild(new Header("Progress bar"));
                    panel.AddChild(new HorizontalLine());
                    panel.AddChild(new Paragraph("Works just like sliders:"));
                    panel.AddChild(new ProgressBar(0, 10));
                }

                // example: lists
                {
                    // create panel and add to list of panels and manager
                    Panel panel = new Panel(new Vector2(450, -1));
                    panels.Add(panel);
                    UserInterface.Active.AddEntity(panel);

                    // list title
                    panel.AddChild(new Header("SelectList"));
                    panel.AddChild(new HorizontalLine());
                    panel.AddChild(new Paragraph("SelectLists let you pick a value from a list of items:"));

                    SelectList list = new SelectList(new Vector2(0, 280));
                    list.AddItem("Warrior");
                    list.AddItem("Mage");
                    list.AddItem("Ranger");
                    list.AddItem("Rogue");
                    list.AddItem("Paladin");
                    list.AddItem("Cleric");
                    list.AddItem("Warlock");
                    list.AddItem("Barbarian");
                    list.AddItem("Monk");
                    panel.AddChild(list);
                }

                // example: lists with icons
                {
                    // create panel and add to list of panels and manager
                    Panel panel = new Panel(new Vector2(450, -1));
                    panels.Add(panel);
                    UserInterface.Active.AddEntity(panel);

                    // list title
                    panel.AddChild(new Header("SelectList with Icons"));
                    panel.AddChild(new HorizontalLine());
                    panel.AddChild(new Paragraph("You can also attach icons to items in lists:"));

                    SelectList list = new SelectList(new Vector2(0, 280));
                    list.AddItem("Warrior");
                    list.AddItem("Mage");
                    list.AddItem("Rogue");
                    list.AddItem("Paladin");
                    list.AddItem("Cleric");
                    list.AddItem("Barbarian");
                    list.AddItem("Necromancer");

                    list.IconsScale *= 1.2f;
                    list.SetIcon("textures/icons/Sword", "Warrior");
                    list.SetIcon("textures/icons/MagicWand", "Mage");
                    list.SetIcon("textures/icons/Trap", "Rogue");
                    list.SetIcon("textures/icons/Axe", "Barbarian");
                    list.SetIcon("textures/icons/MagicBook", "Cleric");
                    list.SetIcon("textures/icons/Helmet", "Paladin");
                    list.SetIcon("textures/icons/Bone", "Necromancer");

                    panel.AddChild(list);
                }

                // example: list as tables
                {
                    // create panel and add to list of panels and manager
                    Panel panel = new Panel(new Vector2(620, -1));
                    panels.Add(panel);
                    UserInterface.Active.AddEntity(panel);

                    // list title
                    panel.AddChild(new Header("SelectList as a Table"));
                    panel.AddChild(new HorizontalLine());
                    panel.AddChild(new Paragraph("With few simple tricks you can also create lists that behave like a table:"));

                    // create the list
                    SelectList list = new SelectList(new Vector2(0, 280));

                    // lock and create title
                    list.LockedItems[0] = true;
                    list.AddItem(System.String.Format("{0}{1,-8} {2,-8} {3, -10}", "{{RED}}", "Name", "Class", "Level"));

                    // add items as formatted table
                    list.AddItem(System.String.Format("{0,-8} {1,-8} {2,-10}", "Joe", "Mage", "5"));
                    list.AddItem(System.String.Format("{0,-8} {1,-8} {2,-10}", "Ron", "Monk", "7"));
                    list.AddItem(System.String.Format("{0,-8} {1,-8} {2,-10}", "Alex", "Rogue", "3"));
                    list.AddItem(System.String.Format("{0,-8} {1,-8} {2,-10}", "Jim", "Paladin", "7"));
                    list.AddItem(System.String.Format("{0,-8} {1,-8} {2,-10}", "Abe", "Cleric", "8"));
                    list.AddItem(System.String.Format("{0,-8} {1,-8} {2,-10}", "James", "Warlock", "20"));
                    list.AddItem(System.String.Format("{0,-8} {1,-8} {2,-10}", "Bob", "Bard", "1"));
                    panel.AddChild(list);
                }

                // example: lists skins
                {
                    // create panel and add to list of panels and manager
                    Panel panel = new Panel(new Vector2(450, -1));
                    panels.Add(panel);
                    UserInterface.Active.AddEntity(panel);

                    // list title
                    panel.AddChild(new Header("SelectList - Skin"));
                    panel.AddChild(new HorizontalLine());
                    panel.AddChild(new Paragraph("Just like panels, SelectList can use alternative skins:"));

                    SelectList list = new SelectList(new Vector2(0, 280), skin: PanelSkin.Alternative);
                    list.AddItem("Warrior");
                    list.AddItem("Mage");
                    list.AddItem("Ranger");
                    list.AddItem("Rogue");
                    list.AddItem("Paladin");
                    list.AddItem("Cleric");
                    list.AddItem("Warlock");
                    list.AddItem("Barbarian");
                    list.AddItem("Monk");
                    panel.AddChild(list);
                }

                // example: dropdown
                {
                    // create panel and add to list of panels and manager
                    Panel panel = new Panel(new Vector2(450, -1));
                    panels.Add(panel);
                    UserInterface.Active.AddEntity(panel);

                    // dropdown title
                    panel.AddChild(new Header("DropDown"));
                    panel.AddChild(new HorizontalLine());

                    panel.AddChild(new Paragraph("DropDown is just like a list, but take less space since it hide the list when not used:"));
                    DropDown drop = new DropDown(new Vector2(0, 250));
                    drop.AddItem("Warrior");
                    drop.AddItem("Mage");
                    drop.AddItem("Ranger");
                    drop.AddItem("Rogue");
                    drop.AddItem("Paladin");
                    drop.AddItem("Cleric");
                    drop.AddItem("Warlock");
                    drop.AddItem("Barbarian");
                    drop.AddItem("Monk");
                    panel.AddChild(drop);

                    panel.AddChild(new Paragraph("And like list, we can set different skins:"));
                    drop = new DropDown(new Vector2(0, 180), skin: PanelSkin.Alternative);
                    drop.AutoSetListHeight = true;
                    drop.AddItem("Warrior");
                    drop.AddItem("Mage");
                    drop.AddItem("Monk");
                    drop.AddItem("Ranger");
                    panel.AddChild(drop);

                    panel.AddChild(new Paragraph("And per-item styling:"));
                    drop = new DropDown(new Vector2(0, 180), skin: PanelSkin.Alternative);
                    drop.AutoSetListHeight = true;
                    drop.AddItem("{{L_RED}}Warrior");
                    drop.AddItem("{{L_BLUE}}Mage");
                    drop.AddItem("{{CYAN}}Monk");
                    drop.AddItem("{{L_GREEN}}Ranger");
                    panel.AddChild(drop);
                }

                // example: panels with scrollbars / overflow
                {
                    // create panel and add to list of panels and manager
                    Panel panel = new Panel(new Vector2(450, 440));
                    panels.Add(panel);
                    UserInterface.Active.AddEntity(panel);

                    // dropdown title
                    panel.AddChild(new Header("Panel Overflow"));
                    panel.AddChild(new HorizontalLine());
                    panel.AddChild(new Paragraph(@"You can choose how to handle entities that overflow parent panel's boundaries. 

The default behavior is to simply overflow (eg entities will be drawn as usual), but you can also make overflowing entities clip, or make the entire panel scrollable. 

In this example, we use a panel with scrollbars.

Note that in order to use clipping and scrollbar with Panels you need to set the UserInterface.Active.UseRenderTarget flag to true.

Here's a button, to test clicking while scrolled:"));
                    panel.AddChild(new Button("a button."));
                    panel.AddChild(new Paragraph(@"And here's a dropdown:"));
                    var dropdown = new DropDown(new Vector2(0, 220));
                    for (int i = 1; i < 10; ++i) dropdown.AddItem("Option" + i.ToString());
                    panel.AddChild(dropdown);
                    panel.AddChild(new Paragraph(@"And a list:"));
                    var list = new SelectList(new Vector2(0, 220));
                    for (int i = 1; i < 10; ++i) list.AddItem("Option" + i.ToString());
                    panel.AddChild(list);
                    panel.PanelOverflowBehavior = PanelOverflowBehavior.VerticalScroll;
                    panel.Scrollbar.AdjustMaxAutomatically = true;
                    panel.Identifier = "panel_with_scrollbar";
                }

                // example: icons
                {
                    // create panel and add to list of panels and manager
                    Panel panel = new Panel(new Vector2(460, -1));
                    panels.Add(panel);
                    UserInterface.Active.AddEntity(panel);

                    // icons title
                    panel.AddChild(new Header("Icons"));
                    panel.AddChild(new HorizontalLine());
                    panel.AddChild(new Paragraph("GeonBit.UI comes with some built-in icons:"));

                    foreach (IconType icon in System.Enum.GetValues(typeof(IconType)))
                    {
                        if (icon == IconType.None)
                        {
                            continue;
                        }
                        panel.AddChild(new Icon(icon, Anchor.AutoInline));
                    }

                    panel.AddChild(new Paragraph("And you can also add an inventory-like frame:"));
                    panel.AddChild(new LineSpace());
                    for (int i = 0; i < 6; ++i)
                    {
                        panel.AddChild(new Icon((IconType)i, Anchor.AutoInline, 1, true));
                    }
                }

                // example: text input
                {
                    // create panel and add to list of panels and manager
                    Panel panel = new Panel(new Vector2(450, -1));
                    panels.Add(panel);
                    UserInterface.Active.AddEntity(panel);

                    // text input example
                    panel.AddChild(new Header("Text Input"));
                    panel.AddChild(new HorizontalLine());

                    // inliner
                    panel.AddChild(new Paragraph("Text input let you get free text from the user:"));
                    TextInput text = new TextInput(false);
                    text.PlaceholderText = "Insert text..";
                    panel.AddChild(text);

                    // multiline
                    panel.AddChild(new Paragraph("Text input can also be multiline, and use different panel skins:"));
                    TextInput textMulti = new TextInput(true, new Vector2(0, 220), skin: PanelSkin.Alternative);
                    textMulti.PlaceholderText = @"Insert multiline text..";
                    panel.AddChild(textMulti);

                    // with hidden password chars
                    panel.AddChild(new Paragraph("Hidden text input:"));
                    TextInput hiddenText = new TextInput(false);
                    hiddenText.PlaceholderText = "Enter password..";
                    hiddenText.HideInputWithChar = '*';
                    panel.AddChild(hiddenText);
                    var hideCheckbox = new CheckBox("Hide password", isChecked: true);
                    hideCheckbox.OnValueChange += (Entity ent) =>
                    {
                        if (hideCheckbox.Checked)
                            hiddenText.HideInputWithChar = '*';
                        else
                            hiddenText.HideInputWithChar = null;
                    };
                    panel.AddChild(hideCheckbox);
                }

                // example: tooltip text
                {
                    // create panel and add to list of panels and manager
                    Panel panel = new Panel(new Vector2(450, -1));
                    panels.Add(panel);
                    UserInterface.Active.AddEntity(panel);

                    // text input example
                    panel.AddChild(new Header("Tooltip Text"));
                    panel.AddChild(new HorizontalLine());

                    // add entity with tooltip text
                    panel.AddChild(new Paragraph(@"You can attach tooltip text to entities.
This text will be shown when the user points on the entity for few seconds. 

For example, try to point on this button:"));
                    var btn = new Button("Button With Tooltip");
                    btn.ToolTipText = @"This is the button tooltip text!
And yes, it can be multiline.";
                    panel.AddChild(btn);
                    panel.AddChild(new Paragraph(@"Note that you can override the function that generates tooltip text entities if you want to create your own custom style."));
                }

                // example: locked text input
                {
                    // create panel and add to list of panels and manager
                    Panel panel = new Panel(new Vector2(500, -1));
                    panels.Add(panel);
                    UserInterface.Active.AddEntity(panel);

                    // text input example
                    panel.AddChild(new Header("Locked Text Input"));
                    panel.AddChild(new HorizontalLine());

                    // inliner
                    panel.AddChild(new Paragraph("A locked multiline text is a cool trick to create long, scrollable text:"));
                    TextInput textMulti = new TextInput(true, new Vector2(0, 370));
                    textMulti.Locked = true;
                    textMulti.TextParagraph.Scale = 0.6f;
                    textMulti.Value = @"The Cleric, Priest, or Bishop is a character class in Dungeons & Dragons and other fantasy role-playing games. 

The cleric is a healer, usually a priest and a holy warrior, originally modeled on or inspired by the Military Orders. 
Clerics are usually members of religious orders, with the original intent being to portray soldiers of sacred orders who have magical abilities, although this role was later taken more clearly by the paladin. 

Most clerics have powers to heal wounds, protect their allies and sometimes resurrect the dead, as well as summon, manipulate and banish undead.

A description of Priests and Priestesses from the Nethack guidebook: Priests and Priestesses are clerics militant, crusaders advancing the cause of righteousness with arms, armor, and arts thaumaturgic. Their ability to commune with deities via prayer occasionally extricates them from peril, but can also put them in it.[1]

A common feature of clerics across many games is that they may not equip pointed weapons such as swords or daggers, and must use blunt weapons such as maces, war-hammers, shields or wand instead. This is based on a popular, but erroneous, interpretation of the depiction of Odo of Bayeux and accompanying text. They are also often limited in what types of armor they can wear, though usually not as restricted as mages.

Related to the cleric is the paladin, who is typically a Lawful Good[citation needed] warrior often aligned with a religious order, and who uses their martial skills to advance its holy cause.";
                    panel.AddChild(textMulti);
                }

                // example: panel tabs
                {
                    // create panel and add to list of panels and manager
                    Panel panel = new Panel(new Vector2(540, 440), skin: PanelSkin.None);
                    panels.Add(panel);
                    UserInterface.Active.AddEntity(panel);

                    // create panel tabs
                    PanelTabs tabs = new PanelTabs();
                    tabs.BackgroundSkin = PanelSkin.Default;
                    panel.AddChild(tabs);

                    // add first panel
                    {
                        TabData tab = tabs.AddTab("Tab 1");
                        tab.panel.AddChild(new Header("PanelTabs"));
                        tab.panel.AddChild(new HorizontalLine());
                        tab.panel.AddChild(new Paragraph(@"PanelTab creates a group of internal panels with toggle buttons to switch between them.

Choose a tab in the buttons above for more info..."));
                    }

                    // add second panel
                    {
                        TabData tab = tabs.AddTab("Tab 2");
                        tab.panel.AddChild(new Header("Tab 2"));
                        tab.panel.AddChild(new HorizontalLine());
                        tab.panel.AddChild(new Paragraph(@"Awesome, you got to tab2!

Maybe something interesting in tab3?"));
                    }

                    // add third panel
                    {
                        TabData tab = tabs.AddTab("Tab 3");
                        tab.panel.AddChild(new Header("Nope."));
                        tab.panel.AddChild(new HorizontalLine());
                        tab.panel.AddChild(new Paragraph("Nothing to see here."));
                    }
                }

                // example: messages
                {
                    // create panel and add to list of panels and manager
                    Panel panel = new Panel(new Vector2(450, -1));
                    panels.Add(panel);
                    UserInterface.Active.AddEntity(panel);

                    // add title and text
                    panel.AddChild(new Header("Message Box"));
                    panel.AddChild(new HorizontalLine());
                    panel.AddChild(new Paragraph("GeonBit.UI comes with a utility to create simple message boxes:"));

                    // button to create simple message box
                    {
                        var btn = new Button("Show Simple Message", ButtonSkin.Default);
                        btn.OnClick += (Entities.Entity entity) =>
                        {
                            Utils.MessageBox.ShowMsgBox("Hello World!", "This is a simple message box. It doesn't say much, really.");
                        };
                        panel.AddChild(btn);
                    }

                    // button to create message box with custombuttons
                    panel.AddChild(new Paragraph("Or you can create custom message and buttons:"));
                    {
                        var btn = new Button("Show Custom Message", ButtonSkin.Default);
                        btn.OnClick += (Entities.Entity entity) =>
                        {
                            Utils.MessageBox.ShowMsgBox("Custom Message!", "In this message there are two custom buttons.\n\nYou can set different actions per button. For example, click on 'Surprise' and see what happens!", new Utils.MessageBox.MsgBoxOption[] {
                                new Utils.MessageBox.MsgBoxOption("Close", () => { return true; }),
                                new Utils.MessageBox.MsgBoxOption("Surprise", () => { Utils.MessageBox.ShowMsgBox("Files Removed Successfully", "Win32 was successfully removed from this computer. Please restart to complete OS destruction."); return true; })
                                });
                        };
                        panel.AddChild(btn);
                    }

                    // button to create message with extras
                    panel.AddChild(new Paragraph("And you can also add extra entities to the message box:"));
                    {
                        var btn = new Button("Message With Extras", ButtonSkin.Default);
                        btn.OnClick += (Entity entity) =>
                        {
                            var textInput = new TextInput(false);
                            textInput.PlaceholderText = "Enter your name";
                            Utils.MessageBox.ShowMsgBox("Message With Extra!", "In this message box we attached an extra entity from outside (a simple text input).\n\nPretty neat, huh?", new Utils.MessageBox.MsgBoxOption[] {
                                new Utils.MessageBox.MsgBoxOption("Close", () => { return true; }),
                                }, new Entity[] { textInput });
                        };
                        panel.AddChild(btn);
                    }
                }

                // example: forms
                {
                    // create panel and add to list of panels and manager
                    Panel panel = new Panel(new Vector2(560, -1));
                    panels.Add(panel);
                    UserInterface.Active.AddEntity(panel);

                    // add title and text
                    panel.AddChild(new Header("UI Forms"));
                    panel.AddChild(new HorizontalLine());
                    panel.AddChild(new Paragraph(@"Forms are objects that help you generate form-like UI groups and easily extract the data of different fields. For example click the button below: 
                    "));

                    // add create form button
                    var btn = panel.AddChild(new Button(@"Show Form"));
                    btn.OnClick += (Entity ent) =>
                    {
                        var newForm = new Form(new FormFieldData[] {
                            new FormFieldData(FormFieldType.TextInput, "text1", "Text Field") { DefaultValue = "Some Default Val" },
                            new FormFieldData(FormFieldType.Slider, "slider1", "Slider Field") { Min = 5, Max = 15, DefaultValue = 10 },
                            new FormFieldData(FormFieldType.RadioButtons, "radios1", "Radio Buttons Field") { Choices = new string[] {"option1", "option2" }, DefaultValue = "option1" },
                            new FormFieldData(FormFieldType.Checkbox, "checkbox1", "Checkbox Field") { },
                            new FormFieldData(FormFieldType.Section, "newsection", "New Form Section") { },
                            new FormFieldData(FormFieldType.DropDown, "dropdown1", "DropDown field") { Choices = new string[] {"option1", "option2", "option3" } },
                        }, null);
                        Utils.MessageBox.ShowMsgBox("Example Form", "", "Close Form And Show Values", extraEntities: new Entity[] { newForm.FormPanel }, onDone: () =>
                        {
                            Utils.MessageBox.ShowMsgBox("Form Values", string.Format(
                                "Text Field: '{5}{0}{6}'\r\n" +
                                "Slider: '{5}{1}{6}'\r\n" +
                                "Radio Buttons: '{5}{2}{6}'\r\n" +
                                "Checkbox: '{5}{3}{6}'\r\n" +
                                "DropDown: '{5}{4}{6}'",
                                newForm.GetValue("text1"),
                                newForm.GetValue("slider1"),
                                newForm.GetValue("radios1"),
                                newForm.GetValue("checkbox1"),
                                newForm.GetValue("dropdown1"),
                                "{{L_GREEN}}", "{{DEFAULT}}"
                                ));
                        });
                    };
                }

                // example: file dialog
                {
                    // create panel and add to list of panels and manager
                    Panel panel = new Panel(new Vector2(560, -1));
                    panels.Add(panel);
                    UserInterface.Active.AddEntity(panel);

                    // add title and text
                    panel.AddChild(new Header("File Dialogs"));
                    panel.AddChild(new HorizontalLine());
                    panel.AddChild(new Paragraph("GeonBit.UI also provide file dialogs. For example, save file dialog: \n"));

                    // add save file button
                    {
                        var btn = panel.AddChild(new Button(@"Open Save File Dialog"));
                        btn.OnClick += (Entity ent) =>
                        {
                            Utils.MessageBox.OpenSaveFileDialog("", (Utils.FileDialogResponse res) =>
                            {
                                Utils.MessageBox.ShowMsgBox("File Selected!", $"Selected file: '{res.FullPath}'.\n\nIn this example we just show a message box, in a real project we would use this path to save the file.");
                                return true;
                            }, message: "It won't actually save anything so don't worry about picking existing files.");
                        };
                    }

                    panel.AddChild(new Paragraph("And click below to open load file dialog: \n"));

                    // add load file button
                    {
                        var btn = panel.AddChild(new Button(@"Open Load File Dialog"));
                        btn.OnClick += (Entity ent) =>
                        {
                            Utils.MessageBox.OpenLoadFileDialog("", (Utils.FileDialogResponse res) =>
                            {
                                Utils.MessageBox.ShowMsgBox("File Selected!", $"Selected file: '{res.FullPath}'.\n\nIn this example we just show a message box, in a real project we would use this path to load the file.");
                                return true;
                            }, message: "It won't actually load anything so don't worry about picking any file.");
                        };
                    }
                }

                // example: top menu
                {
                    // create panel and add to list of panels and manager
                    Panel panel = new Panel(new Vector2(750, -1));
                    panels.Add(panel);
                    UserInterface.Active.AddEntity(panel);

                    // add title and text
                    panel.AddChild(new Header("Top Menu"));
                    panel.AddChild(new HorizontalLine());
                    panel.AddChild(new Paragraph("GeonBit.UI comes with a utility to generate a classic top menu:"));

                    var layout = new Utils.MenuBar.MenuLayout();
                    layout.AddMenu("File", 180);
                    layout.AddItemToMenu("File", "New", () => { Utils.MessageBox.ShowMsgBox("Something New!", "Lets make something new."); });
                    layout.AddItemToMenu("File", "Save", () => { Utils.MessageBox.ShowMsgBox("Something Saved!", "Your thing was saved successfully."); });
                    layout.AddItemToMenu("File", "Load", () => { Utils.MessageBox.ShowMsgBox("Something Loaded!", "Your thing was loaded successfully."); });
                    layout.AddItemToMenu("File", "Exit", () => { Utils.MessageBox.ShowMsgBox("Not Yet", "We still have much to see."); });
                    layout.AddMenu("Display", 220);
                    layout.AddItemToMenu("Display", "Zoom In", () => { UserInterface.Active.GlobalScale += 0.1f; });
                    layout.AddItemToMenu("Display", "Zoom Out", () => { UserInterface.Active.GlobalScale -= 0.1f; });
                    layout.AddItemToMenu("Display", "Reset Zoom", () => { UserInterface.Active.GlobalScale = 1f; });
                    layout.AddMenu("Interactive", 270);
                    layout.AddItemToMenu("Interactive", "Click Me", (Utils.MenuBar.MenuCallbackContext context) => 
                    {
                        context.Entity.ChangeItem(context.ItemIndex, "I was clicked!");
                    });
                    layout.AddItemToMenu("Interactive", "Toggle Me", (Utils.MenuBar.MenuCallbackContext context) =>
                    {
                        context.Entity.Tag = context.Entity.Tag == "on" ? "off" : "on";
                        context.Entity.ChangeItem(context.ItemIndex, (context.Entity.Tag == "on" ? "{{L_GREEN}}" : "") + "Toggle Me");
                    });

                    var menuBar = Utils.MenuBar.Create(layout);
                    menuBar.Anchor = Anchor.Auto;
                    panel.AddChild(menuBar);
                    panel.AddChild(new LineSpace(24));

                    panel.AddChild(new Paragraph("Usually this menu cover the top of the screen and not be inside another panel. Like with most entities in GeonBit.UI, you can also set its skin:"));
                    menuBar = Utils.MenuBar.Create(layout, PanelSkin.Fancy);
                    menuBar.Anchor = Anchor.Auto;
                    panel.AddChild(menuBar);
                }

                // example: disabled
                {
                    // create panel and add to list of panels and manager
                    Panel panel = new Panel(new Vector2(480, 580));
                    panel.Enabled = false;
                    panels.Add(panel);
                    UserInterface.Active.AddEntity(panel);

                    // disabled title
                    panel.AddChild(new Header("Disabled"));
                    panel.AddChild(new HorizontalLine());
                    panel.AddChild(new Paragraph("Entities can be disabled:"));

                    // internal panel
                    Panel panel2 = new Panel(Vector2.Zero, PanelSkin.None, Anchor.Auto);
                    panel2.Padding = Vector2.Zero;
                    panel.AddChild(panel2);
                    panel2.AddChild(new Button("button"));

                    panel2.AddChild(new LineSpace());
                    for (int i = 0; i < 6; ++i)
                    {
                        panel2.AddChild(new Icon((IconType)i, Anchor.AutoInline, 1, true));
                    }
                    panel2.AddChild(new Paragraph("\nDisabled entities are drawn in black & white, and you cannot interact with them.."));

                    SelectList list = new SelectList(new Vector2(0, 130));
                    list.AddItem("Warrior");
                    list.AddItem("Mage");
                    panel2.AddChild(list);
                    panel2.AddChild(new CheckBox("disabled.."));
                }

                // example: Locked
                {
                    // create panel and add to list of panels and manager
                    Panel panel = new Panel(new Vector2(520, 610));
                    panels.Add(panel);
                    UserInterface.Active.AddEntity(panel);

                    // locked title
                    panel.AddChild(new Header("Locked"));
                    panel.AddChild(new HorizontalLine());
                    panel.AddChild(new Paragraph("Entities can also be locked:",
                        Anchor.Auto));

                    Panel panel2 = new Panel(Vector2.Zero, PanelSkin.None, Anchor.Auto);
                    panel2.Padding = Vector2.Zero;
                    panel2.Locked = true;

                    panel.AddChild(panel2);
                    panel2.AddChild(new Button("button"));
                    panel2.AddChild(new LineSpace());

                    for (int i = 0; i < 6; ++i)
                    {
                        panel2.AddChild(new Icon((IconType)i, Anchor.AutoInline, 1, true));
                    }
                    panel2.AddChild(new Paragraph("\nLocked entities will not respond to input, but unlike disabled entities they are drawn normally, eg with colors:"));

                    SelectList list = new SelectList(new Vector2(0, 130));
                    list.AddItem("Warrior");
                    list.AddItem("Mage");
                    panel2.AddChild(list);
                    panel2.AddChild(new CheckBox("locked.."));
                }

                // example: Cursors
                {
                    // create panel and add to list of panels and manager
                    Panel panel = new Panel(new Vector2(450, -1));
                    panels.Add(panel);
                    UserInterface.Active.AddEntity(panel);

                    // add title and text
                    panel.AddChild(new Header("Cursor"));
                    panel.AddChild(new HorizontalLine());
                    panel.AddChild(new Paragraph("GeonBit.UI comes with 3 basic cursor types:"));

                    // default cursor show
                    {
                        Button btn = new Button("Default", ButtonSkin.Default);
                        btn.OnMouseEnter = (Entity entity) => { UserInterface.Active.SetCursor(CursorType.Default); };
                        btn.OnMouseLeave = (Entity entity) => { UserInterface.Active.SetCursor(CursorType.Default); };
                        panel.AddChild(btn);
                    }

                    // pointer cursor show
                    {
                        Button btn = new Button("Pointer", ButtonSkin.Default);
                        btn.OnMouseEnter = (Entity entity) => { UserInterface.Active.SetCursor(CursorType.Pointer); };
                        btn.OnMouseLeave = (Entity entity) => { UserInterface.Active.SetCursor(CursorType.Default); };
                        panel.AddChild(btn);
                    }

                    // ibeam cursor show
                    {
                        Button btn = new Button("IBeam", ButtonSkin.Default);
                        btn.OnMouseEnter = (Entity entity) => { UserInterface.Active.SetCursor(CursorType.IBeam); };
                        btn.OnMouseLeave = (Entity entity) => { UserInterface.Active.SetCursor(CursorType.Default); };
                        panel.AddChild(btn);
                    }

                    panel.AddChild(new Paragraph("And as always, you can also set your own custom cursor:"));
                    {
                        Button btn = new Button("Custom", ButtonSkin.Default);
                        btn.OnMouseEnter = (Entity entity) => { UserInterface.Active.SetCursor(Content.Load<Texture2D>("example/cursor"), 40); };
                        btn.OnMouseLeave = (Entity entity) => { UserInterface.Active.SetCursor(CursorType.Default); };
                        panel.AddChild(btn);
                    }

                }

                // example: Misc
                {
                    // create panel and add to list of panels and manager
                    Panel panel = new Panel(new Vector2(530, -1));
                    panels.Add(panel);
                    UserInterface.Active.AddEntity(panel);

                    // misc title
                    panel.AddChild(new Header("Miscellaneous"));
                    panel.AddChild(new HorizontalLine());
                    panel.AddChild(new Paragraph("Some cool tricks you can do:"));

                    // button with icon
                    Button btn = new Button("Button With Icon");
                    btn.ButtonParagraph.SetAnchorAndOffset(Anchor.CenterLeft, new Vector2(60, 0));
                    btn.AddChild(new Icon(IconType.Book, Anchor.CenterLeft), true);
                    panel.AddChild(btn);
                    
                    // change progressbar color
                    panel.AddChild(new Paragraph("Different ProgressBar colors:"));
                    ProgressBar pb = new ProgressBar();
                    pb.ProgressFill.FillColor = Color.Red;
                    pb.Caption.Text = "Optional caption...";
                    panel.AddChild(pb);

                    // paragraph style with mouse
                    panel.AddChild(new LineSpace());
                    panel.AddChild(new HorizontalLine());
                    Paragraph paragraph = new Paragraph("Hover / click styling..");
                    paragraph.SetStyleProperty("FillColor", new StyleProperty(Color.Purple), EntityState.MouseDown);
                    paragraph.SetStyleProperty("FillColor", new StyleProperty(Color.Red), EntityState.MouseHover);
                    panel.AddChild(paragraph);
                    panel.AddChild(new HorizontalLine());

                    // colored rectangle
                    panel.AddChild(new Paragraph("Colored rectangle:"));
                    ColoredRectangle rect = new ColoredRectangle(Color.Blue, Color.Red, 4, new Vector2(0, 40));
                    panel.AddChild(rect);
                    panel.AddChild(new HorizontalLine());

                    // custom icons
                    panel.AddChild(new Paragraph("Custom icons / images:"));
                    Icon icon = new Icon(IconType.None, Anchor.AutoInline, 1, true, new Vector2(12, 10));
                    icon.Texture = Content.Load<Texture2D>("example/warrior");
                    panel.AddChild(icon);
                    icon = new Icon(IconType.None, Anchor.AutoInline, 1, true, new Vector2(12, 10));
                    icon.Texture = Content.Load<Texture2D>("example/monk");
                    panel.AddChild(icon);
                    icon = new Icon(IconType.None, Anchor.AutoInline, 1, true, new Vector2(12, 10));
                    icon.Texture = Content.Load<Texture2D>("example/mage");
                    panel.AddChild(icon);
                }

                // example: character build page - intro
                {
                    // create panel and add to list of panels and manager
                    Panel panel = new Panel(new Vector2(500, -1));
                    panels.Add(panel);
                    UserInterface.Active.AddEntity(panel);

                    // add title and text
                    panel.AddChild(new Header("Final Example"));
                    panel.AddChild(new HorizontalLine());
                    panel.AddChild(new Paragraph(@"The next example will show a fully-functional character creation page, that use different entities, events, etc.

Click on 'Next' to see the character creation demo."));
                }

                // example: character build page - final
                {
                    int panelWidth = 730;

                    // create panel and add to list of panels and manager
                    Panel panel = new Panel(new Vector2(panelWidth, -1));
                    panels.Add(panel);
                    UserInterface.Active.AddEntity(panel);

                    // add title and text
                    panel.AddChild(new Header("Create New Character"));
                    panel.AddChild(new HorizontalLine());

                    // create an internal panel to align components better - a row that covers the entire width split into 3 columns (left, center, right)
                    // first the container panel
                    Panel entitiesGroup = new Panel(new Vector2(0, 250), PanelSkin.None, Anchor.Auto);
                    entitiesGroup.Padding = Vector2.Zero;
                    panel.AddChild(entitiesGroup);

                    // create grid
                    var columnPanels = Utils.PanelsGrid.GenerateColums(3, entitiesGroup);
                    foreach (var column in columnPanels) { column.Padding = Vector2.Zero; }
                    Panel leftPanel = columnPanels[0];
                    Panel centerPanel = columnPanels[1];
                    Panel rightPanel = columnPanels[2];      

                    // create a character preview panel
                    centerPanel.AddChild(new Label(@"Preview", Anchor.AutoCenter));
                    Panel charPreviewPanel = new Panel(new Vector2(180, 180), PanelSkin.Simple, Anchor.AutoCenter);
                    charPreviewPanel.Padding = Vector2.Zero;
                    centerPanel.AddChild(charPreviewPanel);

                    // create preview pics of character
                    Image previewImage = new Image(Content.Load<Texture2D>("example/warrior"), Vector2.Zero, anchor: Anchor.Center);
                    Image previewImageColor = new Image(Content.Load<Texture2D>("example/warrior_color"), Vector2.Zero, anchor: Anchor.Center);
                    Image previewImageSkin = new Image(Content.Load<Texture2D>("example/warrior_skin"), Vector2.Zero, anchor: Anchor.Center);
                    charPreviewPanel.AddChild(previewImage);
                    charPreviewPanel.AddChild(previewImageColor);
                    charPreviewPanel.AddChild(previewImageSkin);

                    // add skin tone slider
                    Slider skin = new Slider(0, 10, new Vector2(0, -1), SliderSkin.Default, Anchor.Auto);
                    skin.OnValueChange = (Entity entity) =>
                    {
                        Slider slider = (Slider)entity;
                        int alpha = (int)(slider.GetValueAsPercent() * 255);
                        previewImageSkin.FillColor = new Color(60, 32, 25, alpha);
                    };
                    skin.Value = 5;
                    charPreviewPanel.AddChild(skin);

                    // create the class selection list
                    leftPanel.AddChild(new Label(@"Class", Anchor.AutoCenter));
                    SelectList classTypes = new SelectList(new Vector2(0, 208), Anchor.Auto);
                    classTypes.AddItem("Warrior");
                    classTypes.AddItem("Mage");
                    classTypes.AddItem("Ranger");
                    classTypes.AddItem("Monk");
                    classTypes.SelectedIndex = 0;
                    leftPanel.AddChild(classTypes);
                    classTypes.OnValueChange = (Entity entity) =>
                    {
                        string texture = ((SelectList)(entity)).SelectedValue.ToLower();
                        previewImage.Texture = Content.Load<Texture2D>("example/" + texture);
                        previewImageColor.Texture = Content.Load<Texture2D>("example/" + texture + "_color");
                        previewImageSkin.Texture = Content.Load<Texture2D>("example/" + texture + "_skin");
                    };

                    // create color selection buttons
                    rightPanel.AddChild(new Label(@"Color", Anchor.AutoCenter));
                    Color[] colors = { Color.White, Color.Red, Color.Green, Color.Blue, Color.Yellow, Color.Purple, Color.Cyan, Color.Brown, Color.Orange };
                    int colorPickSize = 24;
                    foreach (Color baseColor in colors)
                    {
                        for (int i = 0; i < 9; ++i)
                        {
                            Color color = baseColor * (1.0f - (i * 2 / 16.0f)); color.A = 255;
                            ColoredRectangle currColorButton = new ColoredRectangle(color, Vector2.One * colorPickSize, i == 0 ? Anchor.Auto : Anchor.AutoInlineNoBreak);
                            currColorButton.Padding = currColorButton.SpaceAfter = currColorButton.SpaceBefore = Vector2.Zero;
                            currColorButton.OnClick = (Entity entity) =>
                            {
                                previewImageColor.FillColor = entity.FillColor;
                            };
                            rightPanel.AddChild(currColorButton);
                        }
                    }

                    // gender selection (radio buttons)
                    panel.AddChild(new RadioButton("Male", Anchor.Auto, new Vector2(180, 60), isChecked: true));
                    panel.AddChild(new RadioButton("Female", Anchor.AutoInline, new Vector2(240, 60)));

                    // hardcore mode
                    Button hardcore = new Button("Hardcore", ButtonSkin.Fancy, Anchor.AutoInline, new Vector2(220, 60));
                    hardcore.ButtonParagraph.Scale = 0.8f;
                    hardcore.SpaceBefore = new Vector2(24, 0);
                    hardcore.ToggleMode = true;
                    panel.AddChild(hardcore);
                    panel.AddChild(new HorizontalLine());

                    // add character name, last name, and age
                    // first add the labels
                    panel.AddChild(new Label(@"First Name: ", Anchor.AutoInline, size: new Vector2(0.4f, -1)));
                    panel.AddChild(new Label(@"Last Name: ", Anchor.AutoInline, size: new Vector2(0.4f, -1)));
                    panel.AddChild(new Label(@"Age: ", Anchor.AutoInline, size: new Vector2(0.2f, -1)));

                    // now add the text inputs

                    // first name
                    TextInput firstName = new TextInput(false, new Vector2(0.4f, -1), anchor: Anchor.Auto);
                    firstName.PlaceholderText = "Name";
                    firstName.Validators.Add(new EnglishCharactersOnly(true));
                    firstName.Validators.Add(new OnlySingleSpaces());
                    firstName.Validators.Add(new MakeTitleCase());
                    panel.AddChild(firstName);

                    // last name
                    TextInput lastName = new TextInput(false, new Vector2(0.4f, -1), anchor: Anchor.AutoInline);
                    lastName.PlaceholderText = "Surname";
                    lastName.Validators.Add(new EnglishCharactersOnly(true));
                    lastName.Validators.Add(new OnlySingleSpaces());
                    lastName.Validators.Add(new MakeTitleCase());
                    panel.AddChild(lastName);

                    // age
                    TextInput age = new TextInput(false, new Vector2(0.2f, -1), anchor: Anchor.AutoInline);
                    age.Validators.Add(new NumbersOnly(false, 0, 80));
                    age.Value = "20";
                    age.ValueWhenEmpty = "20";
                    panel.AddChild(age);
                }

                // example: epilogue
                {
                    // create panel and add to list of panels and manager
                    Panel panel = new Panel(new Vector2(520, 400));
                    panels.Add(panel);
                    UserInterface.Active.AddEntity(panel);

                    // add title and text
                    panel.AddChild(new Header("End Of Demo"));
                    panel.AddChild(new HorizontalLine());
                    panel.AddChild(new Paragraph(@"That's it for now! There is still much to learn about GeonBit.UI, but these examples were enough to get you going.

To learn more, please visit the git repo, read the docs, or go through some source code.

If you liked GeonBit.UI feel free to star the repo on GitHub. :)"));
                }

                // init panels and buttons
                UpdateAfterExampleChange();

            }

            // once done init, clear events log
            eventsLog.ClearItems();

            // call base initialize
            base.Initialize();
        }

        /// <summary>
        /// Show next UI example.
        /// </summary>
        public void NextExample()
        {
            currExample++;
            UpdateAfterExampleChange();
        }

        /// <summary>
        /// Show previous UI example.
        /// </summary>
        public void PreviousExample()
        {
            currExample--;
            UpdateAfterExampleChange();
        }

        /// <summary>
        /// Called after we change current example index, to hide all examples
        /// except for the currently active example + disable prev / next buttons if
        /// needed (if first or last example).
        /// </summary>
        protected void UpdateAfterExampleChange()
        {
            // hide all panels and show current example panel
            foreach (Panel panel in panels)
            {
                panel.Visible = false;
            }
            panels[currExample].Visible = true;

            // disable / enable next and previous buttons
            nextExampleButton.Enabled = currExample != panels.Count - 1;
            previousExampleButton.Enabled = currExample != 0;
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // make sure window is focused
            if (!IsActive)
                return;

            // exit on escape
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // update UI
            UserInterface.Active.Update(gameTime);

            // show currently active entity (for testing)
            targetEntityShow.Text = "Target Entity: " + (UserInterface.Active.TargetEntity != null ? UserInterface.Active.TargetEntity.GetType().Name : "null");

            // call base update
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // draw ui
            UserInterface.Active.Draw(spriteBatch);

            // clear buffer
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // finalize ui rendering
            UserInterface.Active.DrawMainRenderTarget(spriteBatch);

            // call base draw function
            base.Draw(gameTime);
        }
    }
}
