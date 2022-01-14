namespace Fabulous.XamarinForms.XFAttributes

open System
open System.Collections.Generic
open Fabulous
open Fabulous.XamarinForms

[<AbstractClass; Sealed>]
type Application =
    static member MainPage =
        Attributes.defineWidget
            "Application_MainPage"
            (fun target -> ViewNode.get (target :?> Xamarin.Forms.Application).MainPage)
            (fun target value -> (target :?> Xamarin.Forms.Application).MainPage <- value)

    static member Resources =
        Attributes.define<Xamarin.Forms.ResourceDictionary>
            "Application_Resources"
            (fun (newValueOpt, node) ->
                let application =
                    node.Target :?> Xamarin.Forms.Application

                let value =
                    match newValueOpt with
                    | ValueNone -> application.Resources
                    | ValueSome v -> v

                application.Resources <- value)

    static member UserAppTheme =
        Attributes.define<Xamarin.Forms.OSAppTheme>
            "Application_UserAppTheme"
            (fun (newValueOpt, node) ->
                let application =
                    node.Target :?> Xamarin.Forms.Application

                let value =
                    match newValueOpt with
                    | ValueNone -> Xamarin.Forms.OSAppTheme.Unspecified
                    | ValueSome v -> v

                application.UserAppTheme <- value)

    static member RequestedThemeChanged =
        Attributes.defineEvent<Xamarin.Forms.AppThemeChangedEventArgs>
            "Application_RequestedThemeChanged"
            (fun target ->
                (target :?> Xamarin.Forms.Application)
                    .RequestedThemeChanged)

    static member ModalPopped =
        Attributes.defineEvent<Xamarin.Forms.ModalPoppedEventArgs>
            "Application_ModalPopped"
            (fun target -> (target :?> Xamarin.Forms.Application).ModalPopped)

    static member ModalPopping =
        Attributes.defineEvent<Xamarin.Forms.ModalPoppingEventArgs>
            "Application_ModalPopping"
            (fun target ->
                (target :?> Xamarin.Forms.Application)
                    .ModalPopping)

    static member ModalPushed =
        Attributes.defineEvent<Xamarin.Forms.ModalPushedEventArgs>
            "Application_ModalPushed"
            (fun target -> (target :?> Xamarin.Forms.Application).ModalPushed)

    static member ModalPushing =
        Attributes.defineEvent<Xamarin.Forms.ModalPushingEventArgs>
            "Application_ModalPushing"
            (fun target ->
                (target :?> Xamarin.Forms.Application)
                    .ModalPushing)

[<AbstractClass; Sealed>]
type Page =
    static member BackgroundImageSource =
        Attributes.defineBindable<Xamarin.Forms.ImageSource> "Page_BackgroundImageSource" Xamarin.Forms.Page.BackgroundImageSourceProperty

    static member IconImageSource =
        Attributes.defineBindable<Xamarin.Forms.ImageSource> "Page_IconImageSource" Xamarin.Forms.Page.IconImageSourceProperty

    static member IsBusy =
        Attributes.defineBindable<bool> "Page_IsBusy" Xamarin.Forms.Page.IsBusyProperty

    static member Padding =
        Attributes.defineBindable<Xamarin.Forms.Thickness> "Page_Padding" Xamarin.Forms.Page.PaddingProperty

    static member Title =
        Attributes.defineBindable<string> "Page_Title" Xamarin.Forms.Page.TitleProperty

    static member ToolbarItems =
        Attributes.defineWidgetCollection<Xamarin.Forms.ToolbarItem>
            "Page_ToolbarItems"
            (fun target -> (target :?> Xamarin.Forms.Page).ToolbarItems)

    static member Appearing =
        Attributes.defineEventNoArg "Page_Appearing" (fun target -> (target :?> Xamarin.Forms.Page).Appearing)

    static member Disappearing =
        Attributes.defineEventNoArg "Page_Disappearing" (fun target -> (target :?> Xamarin.Forms.Page).Disappearing)

    static member LayoutChanged =
        Attributes.defineEventNoArg "Page_LayoutChanged" (fun target -> (target :?> Xamarin.Forms.Page).LayoutChanged)

[<AbstractClass; Sealed>]
type ContentPage =
    static member Content =
        Attributes.defineBindableWidget "Page_Content" Xamarin.Forms.ContentPage.ContentProperty

    static member SizeAllocated =
        Attributes.defineEvent<SizeAllocatedEventArgs>
            "ContentPage_SizeAllocated"
            (fun target -> (target :?> FabulousContentPage).SizeAllocated)

[<AbstractClass; Sealed>]
type Layout =
    static member Padding =
        Attributes.defineBindable<Xamarin.Forms.Thickness> "Layout_Padding" Xamarin.Forms.Layout.PaddingProperty

    static member CascadeInputTransparent =
        Attributes.defineBindable<bool> "Layout_CascadeInputTransparent" Xamarin.Forms.Layout.CascadeInputTransparentProperty

    static member IsClippedToBounds =
        Attributes.defineBindable<bool> "Layout_IsClippedToBounds" Xamarin.Forms.Layout.IsClippedToBoundsProperty

    static member LayoutChanged =
        Attributes.defineEventNoArg
            "Layout_LayoutChanged"
            (fun target -> (target :?> Xamarin.Forms.Layout).LayoutChanged)

[<AbstractClass; Sealed>]
type LayoutOfView =
    static member Children =
        Attributes.defineWidgetCollection
            "LayoutOfWidget_Children"
            (fun target ->
                (target :?> Xamarin.Forms.Layout<Xamarin.Forms.View>)
                    .Children)

[<AbstractClass; Sealed>]
type StackLayout =
    static member Orientation =
        Attributes.defineBindable<Xamarin.Forms.StackOrientation> "StackLayout_Orientation" Xamarin.Forms.StackLayout.OrientationProperty

    static member Spacing =
        Attributes.defineBindable<float> "StackLayout_Spacing" Xamarin.Forms.StackLayout.SpacingProperty

[<AbstractClass; Sealed>]
type Element =
    static member AutomationId =
        Attributes.defineBindable<string> "Element_AutomationId" Xamarin.Forms.Element.AutomationIdProperty

[<AbstractClass; Sealed>]
type VisualElement =
    static member IsEnabled =
        Attributes.defineBindable<bool> "VisualElement_IsEnabled" Xamarin.Forms.VisualElement.IsEnabledProperty

    static member Opacity =
        Attributes.defineBindable<float> "VisualElement_Opacity" Xamarin.Forms.VisualElement.OpacityProperty

    static member BackgroundColor =
        Attributes.defineBindable<Xamarin.Forms.Color> "VisualElement_BackgroundColor" Xamarin.Forms.VisualElement.BackgroundColorProperty

    static member Height =
        Attributes.defineBindable<float> "VisualElement_Height" Xamarin.Forms.VisualElement.HeightRequestProperty

    static member Width =
        Attributes.defineBindable<float> "VisualElement_Width" Xamarin.Forms.VisualElement.WidthRequestProperty

    static member IsVisible =
        Attributes.defineBindable<bool> "VisualElement_IsVisible" Xamarin.Forms.VisualElement.IsVisibleProperty

[<AbstractClass; Sealed>]
type NavigableElement =
    static member Style =
        Attributes.defineBindable<Xamarin.Forms.Style> "NavigableElement_Style" Xamarin.Forms.NavigableElement.StyleProperty

[<AbstractClass; Sealed>]
type View =
    static member HorizontalOptions =
        Attributes.defineBindable<Xamarin.Forms.LayoutOptions> "View_HorizontalOptions" Xamarin.Forms.View.HorizontalOptionsProperty

    static member VerticalOptions =
        Attributes.defineBindable<Xamarin.Forms.LayoutOptions> "View_VerticalOptions" Xamarin.Forms.View.VerticalOptionsProperty

    static member Margin =
        Attributes.defineBindable<Xamarin.Forms.Thickness> "View_Margin" Xamarin.Forms.View.MarginProperty

    static member GestureRecognizers =
        Attributes.defineWidgetCollection<Xamarin.Forms.IGestureRecognizer>
            "View_GestureRecognizers"
            (fun target -> (target :?> Xamarin.Forms.View).GestureRecognizers)

[<AbstractClass; Sealed>]
type Label =
    static member Text =
        Attributes.defineBindable<string> "Label_Text" Xamarin.Forms.Label.TextProperty

    static member HorizontalTextAlignment =
        Attributes.defineBindable<Xamarin.Forms.TextAlignment> "Label_HorizontalTextAlignment" Xamarin.Forms.Label.HorizontalTextAlignmentProperty

    static member VerticalTextAlignment =
        Attributes.defineBindable<Xamarin.Forms.TextAlignment> "Label_VerticalTextAlignment" Xamarin.Forms.Label.VerticalTextAlignmentProperty

    static member FontSize =
        Attributes.defineBindable<double> "Label_FontSize" Xamarin.Forms.Label.FontSizeProperty

    static member Padding =
        Attributes.defineBindable<Xamarin.Forms.Thickness> "Label_Padding" Xamarin.Forms.Label.PaddingProperty

    static member TextColor =
        Attributes.defineBindable<Xamarin.Forms.Color> "Label_TextColor" Xamarin.Forms.Label.TextColorProperty

    static member FontAttributes =
        Attributes.defineBindable<Xamarin.Forms.FontAttributes> "Label_FontAttributes" Xamarin.Forms.Label.FontAttributesProperty

    static member LineBreakMode =
        Attributes.defineBindable<Xamarin.Forms.LineBreakMode> "Label_LineBreakMode" Xamarin.Forms.Label.LineBreakModeProperty

[<AbstractClass; Sealed>]
type Button =
    static member Text =
        Attributes.defineBindable<string> "Button_Text" Xamarin.Forms.Button.TextProperty

    static member Clicked =
        Attributes.defineEventNoArg "Button_Clicked" (fun target -> (target :?> Xamarin.Forms.Button).Clicked)

    static member TextColor =
        Attributes.defineAppThemeBindable<Xamarin.Forms.Color> "Button_TextColor" Xamarin.Forms.Button.TextColorProperty

    static member FontSize =
        Attributes.defineBindable<double> "Button_FontSize" Xamarin.Forms.Button.FontSizeProperty

[<AbstractClass; Sealed>]
type ImageButton =
    static member Source =
        Attributes.defineBindable<Xamarin.Forms.ImageSource> "ImageButton_Source" Xamarin.Forms.ImageButton.SourceProperty

    static member Aspect =
        Attributes.defineBindable<Xamarin.Forms.Aspect> "ImageButton_Aspect" Xamarin.Forms.ImageButton.AspectProperty

    static member Clicked =
        Attributes.defineEventNoArg "ImageButton_Clicked" (fun target -> (target :?> Xamarin.Forms.ImageButton).Clicked)

[<AbstractClass; Sealed>]
type Switch =
    static member IsToggled =
        Attributes.defineBindable<bool> "Switch_IsToggled" Xamarin.Forms.Switch.IsToggledProperty

    static member Toggled =
        Attributes.defineEvent<Xamarin.Forms.ToggledEventArgs>
            "Switch_Toggled"
            (fun target -> (target :?> Xamarin.Forms.Switch).Toggled)

[<AbstractClass; Sealed>]
type Slider =
    static member MinimumMaximum =
        Attributes.define<float * float> "Slider_MinimumMaximum" ViewUpdaters.updateSliderMinMax

    static member Value =
        Attributes.defineBindable<float> "Slider_Value" Xamarin.Forms.Slider.ValueProperty

    static member ValueChanged =
        Attributes.defineEvent<Xamarin.Forms.ValueChangedEventArgs>
            "Slider_ValueChanged"
            (fun target -> (target :?> Xamarin.Forms.Slider).ValueChanged)

[<AbstractClass; Sealed>]
type ActivityIndicator =
    static member IsRunning =
        Attributes.defineBindable<bool> "ActivityIndicator_IsRunning" Xamarin.Forms.ActivityIndicator.IsRunningProperty

[<AbstractClass; Sealed>]
type ContentView =
    static member Content =
        Attributes.defineBindableWidget "ContentView_Content" Xamarin.Forms.ContentView.ContentProperty

[<AbstractClass; Sealed>]
type RefreshView =
    static member IsRefreshing =
        Attributes.defineBindable<bool> "RefreshView_IsRefreshing" Xamarin.Forms.RefreshView.IsRefreshingProperty

    static member Refreshing =
        Attributes.defineEventNoArg
            "RefreshView_Refreshing"
            (fun target -> (target :?> Xamarin.Forms.RefreshView).Refreshing)

[<AbstractClass; Sealed>]
type ScrollView =
    static member Content =
        Attributes.defineWidget
            "ScrollView_Content"
            (fun target -> ViewNode.get (target :?> Xamarin.Forms.ScrollView).Content)
            (fun target value -> (target :?> Xamarin.Forms.ScrollView).Content <- value)

[<AbstractClass; Sealed>]
type Image =
    static member Source =
        Attributes.defineBindable<Xamarin.Forms.ImageSource> "Image_Source" Xamarin.Forms.Image.SourceProperty

    static member Aspect =
        Attributes.defineBindable<Xamarin.Forms.Aspect> "Image_Aspect" Xamarin.Forms.Image.AspectProperty

[<AbstractClass; Sealed>]
type Grid =
    static member ColumnDefinitions =
        Attributes.defineScalarWithConverter<seq<Dimension>, Dimension array, Dimension array>
            "Grid_ColumnDefinitions"
            Array.ofSeq
            id
            ScalarAttributeComparers.equalityCompare
            ViewUpdaters.updateGridColumnDefinitions

    static member RowDefinitions =
        Attributes.defineScalarWithConverter<seq<Dimension>, Dimension array, Dimension array>
            "Grid_RowDefinitions"
            Array.ofSeq
            id
            ScalarAttributeComparers.equalityCompare
            ViewUpdaters.updateGridRowDefinitions

    static member Column =
        Attributes.defineBindable<int> "Grid_Column" Xamarin.Forms.Grid.ColumnProperty

    static member Row =
        Attributes.defineBindable<int> "Grid_Row" Xamarin.Forms.Grid.RowProperty

    static member ColumnSpacing =
        Attributes.defineBindable<float> "Grid_ColumnSpacing" Xamarin.Forms.Grid.ColumnSpacingProperty

    static member RowSpacing =
        Attributes.defineBindable<float> "Grid_RowSpacing" Xamarin.Forms.Grid.RowSpacingProperty

    static member ColumnSpan =
        Attributes.defineBindable<int> "Grid_ColumnSpan" Xamarin.Forms.Grid.ColumnSpanProperty

    static member RowSpan =
        Attributes.defineBindable<int> "Grid_RowSpan" Xamarin.Forms.Grid.RowSpanProperty

[<AbstractClass; Sealed>]
type BoxView =
    static member Color =
        Attributes.defineBindable<Xamarin.Forms.Color> "BoxView_Color" Xamarin.Forms.BoxView.ColorProperty

[<AbstractClass; Sealed>]
type NavigationPage =
    static member Pages =
        Attributes.defineWidgetCollectionWithConverter
            "NavigationPage_Pages"
            ViewUpdaters.applyDiffNavigationPagePages
            ViewUpdaters.updateNavigationPagePages

    static member BarBackgroundColor =
        Attributes.defineBindable<Xamarin.Forms.Color> "NavigationPage_BarBackgroundColor" Xamarin.Forms.NavigationPage.BarBackgroundColorProperty

    static member BarTextColor =
        Attributes.defineBindable<Xamarin.Forms.Color> "NavigationPage_BarTextColor" Xamarin.Forms.NavigationPage.BarTextColorProperty

    static member HasNavigationBar =
        Attributes.defineBindable<bool> "NavigationPage_HasNavigationBar" Xamarin.Forms.NavigationPage.HasNavigationBarProperty

    static member HasBackButton =
        Attributes.defineBindable<bool> "NavigationPage_HasBackButton" Xamarin.Forms.NavigationPage.HasBackButtonProperty

    static member Popped =
        Attributes.defineEvent<Xamarin.Forms.NavigationEventArgs>
            "NavigationPage_Popped"
            (fun target -> (target :?> Xamarin.Forms.NavigationPage).Popped)

[<AbstractClass; Sealed>]
type Entry =
    static member Text =
        Attributes.defineBindable<string> "Entry_Text" Xamarin.Forms.Entry.TextProperty

    static member TextChanged =
        Attributes.defineEvent<Xamarin.Forms.TextChangedEventArgs>
            "Entry_TextChanged"
            (fun target -> (target :?> Xamarin.Forms.Entry).TextChanged)

    static member Placeholder =
        Attributes.defineBindable<string> "Entry_Placeholder" Xamarin.Forms.Entry.PlaceholderProperty

    static member Keyboard =
        Attributes.defineBindable<Xamarin.Forms.Keyboard> "Entry_Keyboard" Xamarin.Forms.Entry.KeyboardProperty

[<AbstractClass; Sealed>]
type TapGestureRecognizer =
    static member Tapped =
        Attributes.defineEventNoArg
            "TapGestureRecognizer_Tapped"
            (fun target ->
                (target :?> Xamarin.Forms.TapGestureRecognizer)
                    .Tapped)

[<AbstractClass; Sealed>]
type SearchBar =
    static member Text =
        Attributes.defineBindable<string> "SearchBar_Text" Xamarin.Forms.SearchBar.TextProperty

    static member CancelButtonColor =
        Attributes.defineBindable<Xamarin.Forms.Color> "SearchBar_CancelButtonColor" Xamarin.Forms.SearchBar.CancelButtonColorProperty

    static member SearchButtonPressed =
        Attributes.defineEventNoArg
            "SearchBar_SearchButtonPressed"
            (fun target ->
                (target :?> Xamarin.Forms.SearchBar)
                    .SearchButtonPressed)

[<AbstractClass; Sealed>]
type InputView =
    static member TextChanged =
        Attributes.defineEvent<Xamarin.Forms.TextChangedEventArgs>
            "InputView_TextChanged"
            (fun target -> (target :?> Xamarin.Forms.InputView).TextChanged)

[<AbstractClass; Sealed>]
type MenuItem =
    static member Text =
        Attributes.defineBindable<string> "MenuItem_Text" Xamarin.Forms.MenuItem.TextProperty

    static member Clicked =
        Attributes.defineEventNoArg "MenuItem_Clicked" (fun target -> (target :?> Xamarin.Forms.MenuItem).Clicked)

[<AbstractClass; Sealed>]
type ToolbarItem =
    static member Order =
        Attributes.define<Xamarin.Forms.ToolbarItemOrder>
            "ToolbarItem_Order"
            (fun (newValueOpt, node) ->
                let toolbarItem =
                    node.Target :?> Xamarin.Forms.ToolbarItem

                match newValueOpt with
                | ValueNone -> toolbarItem.Order <- Xamarin.Forms.ToolbarItemOrder.Default
                | ValueSome order -> toolbarItem.Order <- order)

[<AbstractClass; Sealed>]
type Editor =
    static member Text =
        Attributes.defineBindable<string> "Editor_Text" Xamarin.Forms.Editor.TextProperty

[<AbstractClass; Sealed>]
type ViewCell =
    static member View =
        Attributes.defineWidget
            "ViewCell_View"
            (fun target -> ViewNode.get (target :?> Xamarin.Forms.ViewCell).View)
            (fun target value -> (target :?> Xamarin.Forms.ViewCell).View <- value)

[<AbstractClass; Sealed>]
type MultiPageOfPage =
    static member Children =
        Attributes.defineWidgetCollection
            "MultiPageOfPage"
            (fun target ->
                (target :?> Xamarin.Forms.MultiPage<Xamarin.Forms.Page>)
                    .Children)

[<AbstractClass; Sealed>]
type DatePicker =
    static member CharacterSpacing =
        Attributes.defineBindable<float> "DatePicker_CharacterSpacing" Xamarin.Forms.DatePicker.CharacterSpacingProperty

    static member Date =
        Attributes.defineBindable<DateTime> "DatePicker_Date" Xamarin.Forms.DatePicker.DateProperty

    static member FontAttributes =
        Attributes.defineBindable<Xamarin.Forms.FontAttributes> "DatePicker_FontAttributes" Xamarin.Forms.DatePicker.FontAttributesProperty

    static member FontFamily =
        Attributes.defineBindable<string> "DatePicker_FontFamily" Xamarin.Forms.DatePicker.FontFamilyProperty

    static member FontSize =
        Attributes.defineBindable<float> "DatePicker_FontSize" Xamarin.Forms.DatePicker.FontSizeProperty

    static member Format =
        Attributes.defineBindable<string> "DatePicker_Format" Xamarin.Forms.DatePicker.FormatProperty

    static member MaximumDate =
        Attributes.defineBindable<DateTime> "DatePicker_MaximumDate" Xamarin.Forms.DatePicker.MaximumDateProperty

    static member MinimumDate =
        Attributes.defineBindable<DateTime> "DatePicker_MinimumDate" Xamarin.Forms.DatePicker.MinimumDateProperty

    static member TextColor =
        Attributes.defineAppThemeBindable<Xamarin.Forms.Color> "DatePicker_TextColor" Xamarin.Forms.DatePicker.TextColorProperty

    static member TextTransform =
        Attributes.defineBindable<Xamarin.Forms.TextTransform> "DatePicker_TextTransform" Xamarin.Forms.DatePicker.TextTransformProperty

    static member DateSelected =
        Attributes.defineEvent<Xamarin.Forms.DateChangedEventArgs>
            "DatePicker_DateSelected"
            (fun target -> (target :?> Xamarin.Forms.DatePicker).DateSelected)

[<AbstractClass; Sealed>]
type TimePicker =
    static member CharacterSpacing =
        Attributes.defineBindable<float> "TimePicker_CharacterSpacing" Xamarin.Forms.TimePicker.CharacterSpacingProperty

    static member Time =
        Attributes.defineBindable<TimeSpan> "TimePicker_Time" Xamarin.Forms.TimePicker.TimeProperty

    static member TimeSelected =
        Attributes.defineEvent "TimePicker_TimeSelected" (fun target -> (target :?> FabulousTimePicker).TimeSelected)

    static member FontAttributes =
        Attributes.defineBindable<Xamarin.Forms.FontAttributes> "TimePicker_FontAttributes" Xamarin.Forms.TimePicker.FontAttributesProperty

    static member FontFamily =
        Attributes.defineBindable<string> "TimePicker_FontFamily" Xamarin.Forms.TimePicker.FontFamilyProperty

    static member FontSize =
        Attributes.defineBindable<float> "TimePicker_FontSize" Xamarin.Forms.TimePicker.FontSizeProperty

    static member Format =
        Attributes.defineBindable<string> "TimePicker_Format" Xamarin.Forms.TimePicker.FormatProperty

    static member TextColor =
        Attributes.defineAppThemeBindable<Xamarin.Forms.Color> "TimePicker_TextColor" Xamarin.Forms.TimePicker.TextColorProperty

    static member TextTransform =
        Attributes.defineBindable<Xamarin.Forms.TextTransform> "TimePicker_TextTransform" Xamarin.Forms.TimePicker.TextTransformProperty

[<AbstractClass; Sealed>]
type Stepper =
    static member Increment =
        Attributes.defineBindable<float> "Stepper_Increment" Xamarin.Forms.Stepper.IncrementProperty

    static member MinimumMaximum =
        Attributes.define<float * float> "Stepper_MinimumMaximum" ViewUpdaters.updateStepperMinMax

    static member Value =
        Attributes.defineBindable<float> "Stepper_Value" Xamarin.Forms.Stepper.ValueProperty

    static member ValueChanged =
        Attributes.defineEvent<Xamarin.Forms.ValueChangedEventArgs>
            "Stepper_ValueChanged"
            (fun target -> (target :?> Xamarin.Forms.Stepper).ValueChanged)

[<AbstractClass; Sealed>]
type ItemsView =
    static member ItemsSource<'T>() =
        Attributes.defineBindableWithComparer<WidgetItems<'T>, WidgetItems<'T>, IEnumerable<Widget>>
            $"ItemsView_ItemsSource<{typeof<'T>.Name}"
            Xamarin.Forms.ItemsView.ItemsSourceProperty
            id
            (fun modelValue ->
                seq {
                    for x in modelValue.OriginalItems do
                        modelValue.Template x
                })
            (fun (a, b) -> ScalarAttributeComparers.equalityCompare (a.OriginalItems, b.OriginalItems))

    static member GroupedItemsSource<'T>() =
        Attributes.defineBindableWithComparer<GroupedWidgetItems<'T>, GroupedWidgetItems<'T>, IEnumerable<GroupItem>>
            $"ItemsView_GroupedItemsSource<{typeof<'T>}"
            Xamarin.Forms.ItemsView.ItemsSourceProperty
            id
            (fun modelValue ->
                seq {
                    for x in modelValue.OriginalItems do
                        modelValue.Template x
                })
            (fun (a, b) -> ScalarAttributeComparers.equalityCompare (a.OriginalItems, b.OriginalItems))

[<AbstractClass; Sealed>]
type ItemsViewOfCell =
    static member ItemsSource<'T>() =
        Attributes.defineBindableWithComparer<WidgetItems<'T>, WidgetItems<'T>, IEnumerable<Widget>>
            $"ItemsViewOfCell_ItemsSource<{typeof<'T>}>"
            Xamarin.Forms.ItemsView<Xamarin.Forms.Cell>
                .ItemsSourceProperty
            id
            (fun modelValue ->
                seq {
                    for x in modelValue.OriginalItems do
                        modelValue.Template x
                })
            (fun (a, b) -> ScalarAttributeComparers.equalityCompare (a.OriginalItems, b.OriginalItems))

    static member GroupedItemsSource<'T>() =
        Attributes.defineBindableWithComparer<GroupedWidgetItems<'T>, GroupedWidgetItems<'T>, IEnumerable<GroupItem>>
            $"ItemsViewOfCell_ItemsSource<{typeof<'T>}>"
            Xamarin.Forms.ItemsView<Xamarin.Forms.Cell>
                .ItemsSourceProperty
            id
            (fun modelValue ->
                seq {
                    for x in modelValue.OriginalItems do
                        modelValue.Template x
                })
            (fun (a, b) -> ScalarAttributeComparers.equalityCompare (a.OriginalItems, b.OriginalItems))

[<AbstractClass; Sealed>]
type ListView =
    static member RowHeight =
        Attributes.defineBindable<int> "ListView_RowHeight" Xamarin.Forms.ListView.RowHeightProperty

    static member SelectionMode =
        Attributes.defineBindable<Xamarin.Forms.ListViewSelectionMode> "ListView_SelectionMode" Xamarin.Forms.ListView.SelectionModeProperty

    static member ItemTapped =
        Attributes.defineEvent "ListView_ItemTapped" (fun target -> (target :?> Xamarin.Forms.ListView).ItemTapped)


[<AbstractClass; Sealed>]
type TextCell =
    static member Text =
        Attributes.defineBindable<string> "TextCell_Text" Xamarin.Forms.TextCell.TextProperty

    static member TextColor =
        Attributes.defineBindable<Xamarin.Forms.Color> "TextCell_TextColor" Xamarin.Forms.TextCell.TextColorProperty

[<AbstractClass; Sealed>]
type CollectionView =
    static member RemainingItemsThreshold =
        Attributes.defineBindable<int> "CollectionView_RemainingItemsThreshold" Xamarin.Forms.CollectionView.RemainingItemsThresholdProperty

    static member RemainingItemsThresholdReached =
        Attributes.defineEventNoArg
            "CollectionView_RemainingItemsThresholdReached"
            (fun target ->
                (target :?> Xamarin.Forms.CollectionView)
                    .RemainingItemsThresholdReached)
