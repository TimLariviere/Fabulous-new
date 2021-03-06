namespace Fabulous.XamarinForms

open System.Runtime.CompilerServices
open Fabulous
open Fabulous.XamarinForms
open Xamarin.Forms
open Xamarin.Forms.Shapes

type IArcSegment =
    inherit IPathSegment

module ArcSegment =
    let WidgetKey = Widgets.register<ArcSegment> ()

    let IsLargeArc =
        Attributes.defineBindable<bool> ArcSegment.IsLargeArcProperty

    let Point =
        Attributes.defineBindable<Point> ArcSegment.SizeProperty

    let RotationAngle =
        Attributes.defineBindable<float> ArcSegment.RotationAngleProperty

    let Size =
        Attributes.defineBindable<Size> ArcSegment.SizeProperty

    let SweepDirection =
        Attributes.defineBindable<SweepDirection> ArcSegment.SweepDirectionProperty

[<AutoOpen>]
module ArcSegmentBuilders =

    type Fabulous.XamarinForms.View with
        static member inline ArcSegment<'msg>(point: Point, size: Size) =
            WidgetBuilder<'msg, IArcSegment>(
                ArcSegment.WidgetKey,
                ArcSegment.Point.WithValue(point),
                ArcSegment.Size.WithValue(size)
            )

[<Extension>]
type ArcSegmentModifiers =

    [<Extension>]
    static member inline rotationAngle(this: WidgetBuilder<'msg, #IArcSegment>, value: float) =
        this.AddScalar(ArcSegment.RotationAngle.WithValue(value))

    [<Extension>]
    static member inline sweepDirection(this: WidgetBuilder<'msg, #IArcSegment>, value: SweepDirection) =
        this.AddScalar(ArcSegment.SweepDirection.WithValue(value))

    [<Extension>]
    static member inline isLargeArc(this: WidgetBuilder<'msg, #IArcSegment>, value: bool) =
        this.AddScalar(ArcSegment.IsLargeArc.WithValue(value))
