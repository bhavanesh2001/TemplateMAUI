﻿using Microsoft.Maui.Controls.Shapes;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace TemplateMAUI.Controls
{
    /// <summary>
    /// The Button is a custom templated control that implements the IButton interface. 
    /// It represents a button control with customizable content, appearance, and behavior, making it a versatile component for user interaction in your application.
    /// </summary>
    [ContentProperty(nameof(Content))]
    public class Button : TemplatedView, IButton
    {
        const string ElementFeedback = "PART_Feedback";
        const string ElementContainer = "PART_Container";

        FeedbackView _feedbackView;
        Border _container;

        TapGestureRecognizer _tapGestureRecognizer;
        PointerGestureRecognizer _pointerGestureRecognizer;

        ButtonVisualState _visualState;

        public static new readonly BindableProperty BackgroundProperty = ButtonBase.BackgroundProperty;

        public new Brush Background
        {
            get => (Brush)GetValue(BackgroundProperty);
            set => SetValue(BackgroundProperty, value);
        }

        public static readonly BindableProperty BorderBrushProperty = ButtonBase.BorderBrushProperty;

        public Brush BorderBrush
        {
            get => (Brush)GetValue(BorderBrushProperty);
            set => SetValue(BorderBrushProperty, value);
        }

        public static readonly BindableProperty BorderThicknessProperty = ButtonBase.BorderThicknessProperty;

        public double BorderThickness
        {
            get => (double)GetValue(BorderThicknessProperty);
            set => SetValue(BorderThicknessProperty, value);
        }

        public static readonly BindableProperty CornerRadiusProperty =
            BindableProperty.Create(nameof(CornerRadius), typeof(CornerRadius), typeof(Button), new CornerRadius(6d),
                propertyChanged: OnCornerRadiusChanged);

        static void OnCornerRadiusChanged(BindableObject bindable, object oldValue, object newValue)
        {
            (bindable as Button).UpdateCornerRadius();
        }

        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        public static readonly BindableProperty TextColorProperty = ButtonBase.TextColorProperty;

        public Color TextColor
        {
            get => (Color)GetValue(TextColorProperty);
            set => SetValue(TextColorProperty, value);
        }

        public static BindableProperty FontSizeProperty = ButtonBase.FontSizeProperty;

        [TypeConverter(typeof(FontSizeConverter))]
        public double FontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        public static BindableProperty FontFamilyProperty = ButtonBase.FontFamilyProperty;

        public string FontFamily
        {
            get { return (string)GetValue(FontFamilyProperty); }
            set { SetValue(FontFamilyProperty, value); }
        }

        public static BindableProperty FontAttributesProperty = ButtonBase.FontAttributesProperty;

        public FontAttributes FontAttributes
        {
            get { return (FontAttributes)GetValue(FontAttributesProperty); }
            set { SetValue(FontAttributesProperty, value); }
        }

        public static readonly BindableProperty RippleColorProperty = ButtonBase.RippleColorProperty;

        public Color RippleColor
        {
            get => (Color)GetValue(RippleColorProperty);
            set => SetValue(RippleColorProperty, value);
        }

        public static readonly BindableProperty ContentProperty = ButtonBase.ContentProperty;

        public object Content
        {
            get
            {
                var value = GetValue(ContentProperty);

                if (value is not null && value.GetType() == typeof(string))
                {
                    return ContentAsString(value);
                }

                return value;
            }
            set { SetValue(ContentProperty, value); }
        }

        public static readonly BindableProperty CommandProperty = ButtonBase.CommandProperty;

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set { SetValue(CommandProperty, value); }
        }

        public static readonly BindableProperty CommandParameterProperty = ButtonBase.CommandParameterProperty;

        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set { SetValue(CommandParameterProperty, value); }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public ButtonVisualState ButtonVisualState
        {
            get => _visualState;
            set
            {
                _visualState = value;
                ChangeVisualState();
            }
        }

        public event EventHandler Pressed;
        public event EventHandler Released;
        public event EventHandler PointerEntered;
        public event EventHandler PointerExited;
        public event EventHandler Clicked;

        public View ContentAsString(object content)
        {
            return new Label
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                FontFamily = FontFamily,
                FontSize = FontSize,
                FontAttributes = FontAttributes,
                TextColor = TextColor,
                Text = content?.ToString()
            };
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _feedbackView = GetTemplateChild(ElementFeedback) as FeedbackView;
            _container = GetTemplateChild(ElementContainer) as Border;

            _tapGestureRecognizer = new TapGestureRecognizer();
            _pointerGestureRecognizer = new PointerGestureRecognizer();

            UpdateIsEnabled();
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == IsEnabledProperty.PropertyName)
                UpdateIsEnabled();
        }

        protected override void ChangeVisualState()
        {
            string state;

            switch (ButtonVisualState)
            {
                case ButtonVisualState.Normal:
                    state = "Normal";
                    break;
                case ButtonVisualState.MouseOver:
                    state = "MouseOver";
                    break;
                case ButtonVisualState.Pressed:
                    state = "Pressed";
                    break;
                case ButtonVisualState.Disabled:
                    state = "Disabled";
                    break;
                default:
                    state = "Normal";
                    break;
            }

            VisualStateManager.GoToState(this, state);
        }

        void UpdateIsEnabled()
        {
            if (IsEnabled)
            {
                _tapGestureRecognizer.Tapped += OnButtonTapped;
                _container.GestureRecognizers.Add(_tapGestureRecognizer);

                _pointerGestureRecognizer.PointerEntered += OnButtonPointerEntered;
                _pointerGestureRecognizer.PointerPressed += OnButtonPointerPressed;
                _pointerGestureRecognizer.PointerMoved += OnButtonPointerMoved;
                _pointerGestureRecognizer.PointerExited += OnButtonHandlePointerExited;
                _pointerGestureRecognizer.PointerReleased += OnButtonPointerReleased;
                _container.GestureRecognizers.Add(_pointerGestureRecognizer);
            }
            else
            {
                if (_tapGestureRecognizer is not null)
                {
                    _tapGestureRecognizer.Tapped -= OnButtonTapped;
                    _container.GestureRecognizers.Remove(_tapGestureRecognizer);
                }

                if (_pointerGestureRecognizer is not null)
                {
                    _pointerGestureRecognizer.PointerEntered -= OnButtonPointerEntered;
                    _pointerGestureRecognizer.PointerPressed -= OnButtonPointerPressed;
                    _pointerGestureRecognizer.PointerMoved -= OnButtonPointerMoved;
                    _pointerGestureRecognizer.PointerExited -= OnButtonHandlePointerExited;
                    _pointerGestureRecognizer.PointerReleased -= OnButtonPointerReleased;
                    _container.GestureRecognizers.Remove(_pointerGestureRecognizer);
                }
            }

            ButtonVisualState = IsEnabled ? ButtonVisualState.Normal : ButtonVisualState.Disabled;
        }

        void UpdateCornerRadius()
        {
            if (_feedbackView is not null)
                _feedbackView.CornerRadius = CornerRadius;

            if (_container is not null && _container.StrokeShape is RoundRectangle containerStrokeShape)
                containerStrokeShape.CornerRadius = CornerRadius;        
        }

        void OnButtonTapped(object sender, TappedEventArgs e)
        {
            Clicked?.Invoke(this, EventArgs.Empty);

            if (Command is not null && Command.CanExecute(CommandParameter))
                Command.Execute(null);
        }

        void OnButtonPointerEntered(object sender, PointerEventArgs e)
        {
            PointerEntered?.Invoke(this, EventArgs.Empty);
        }

        void OnButtonPointerPressed(object sender, PointerEventArgs e)
        {
            UpdateVisualState(ButtonVisualState.Pressed);
            Pressed?.Invoke(this, EventArgs.Empty);
        }

        void OnButtonPointerMoved(object sender, PointerEventArgs e)
        {
            UpdateVisualState(ButtonVisualState.MouseOver);
        }

        void OnButtonHandlePointerExited(object sender, PointerEventArgs e)
        {
            UpdateVisualState(ButtonVisualState.Normal);
            PointerExited?.Invoke(this, EventArgs.Empty);
        }

        void OnButtonPointerReleased(object sender, PointerEventArgs e)
        {
            Released?.Invoke(this, EventArgs.Empty);
        }

        void UpdateVisualState(ButtonVisualState visualState)
        {
            if (!IsEnabled)
                return;

            ButtonVisualState = visualState;
        }
    }
}