{% include_relative _header.md %}

{% include_relative contents.md %}

Models
--------
##### `topic last updated: v1.0 - 04.04.2021 - 02:51pm`
<br /> 

The model is the core data from which the whole state of the app can be resurrected.  The model is generally immutable but may also contain elements such as service connections.
It is common for the design of the model to grow "organically" as you prototype your app.

The init function returns your initial model.  The update function updates the model as messages are received.

### Messages and Validation

Validation is generally done on updates to the model storing error messages from validation logic in the model so they can be correctly and simply displayed to the user. Here is a very basic example:

```fsharp
type Animal =
    | ValidAnimal of string
    | InvalidAnimal of string

type Model = { AnimalName : Animal }

type Msg = UpdateAnimal of string

let validAnimalNames = [ "Emu"; "Kangaroo"; "Platypus"; "Wombat" ]

let validateAnimal (animalName : string) =
    if List.contains animalName validAnimalNames
    then ValidAnimal animalName
    else InvalidAnimal animalName

let update msg model =
    match msg with
    | UpdateAnimal animalName -> { model with AnimalName = validateAnimal animalName }

let view (model: Model) : ViewElement =
    let makeEntryCell text =
        Entry(text, UpdateAnimal)
    
    ContentPage(
        VerticalStackLayout(
            match model.AnimalName with
            | ValidAnimal validName -> [ makeEntryCell validName ]
            | InvalidAnimal invalidName ->
                [ makeEntryCell invalidName
                    Label(sprintf "%s is not a valid animal name. Try %A" invalidName validAnimalNames) ]
        )
    )

let init () = { AnimalName = validateAnimal "Emu" }
```

A more advanced validation might use the `Result<'T,'TError>` type to wrap parts of the model that require validation: in the previous example the `Result` type has somewhat been reinvented. Using `Result` provides a consistent way of knowing which parts of the model are in a valid state, use of the standard `Result` functions like `map` and `bind` to perform branching logic, and more comprehensive error messaging. One thing to note is that `'TError` will usually need to carry the original input value so it can be displayed back to the user.

```fsharp
type Animal = Animal of string

type ErrorMessage =
    | InvalidName of InputString : string
    | BlankName

type Model = { AnimalName : Result<Animal,ErrorMessage> }

type Msg = UpdateAnimal of string

let validAnimalNames = [ "Emu"; "Kangaroo"; "Platypus"; "Wombat" ]

let validateAnimal (animalName : string) =
    if animalName = ""
    then Error BlankName
    else
        if List.contains animalName validAnimalNames
        then Ok (Animal animalName)
        else Error (InvalidName animalName)

let update msg model =
    match msg with
    | UpdateAnimal animalName -> { model with AnimalName = validateAnimal animalName }

let view (model: Model) : ViewElement =
    let makeEntryCell text =
        Entry(text, UpdateAnimal)

    let makeErrorMsg err =
        match err with
        | InvalidName invalidName ->
            [ makeEntryCell invalidName
                Label(sprintf "%s is not a valid animal name. Try %A" invalidName validAnimalNames) ]
        | BlankName ->
            [ makeEntryCell ""
                Label("You must input a name") ]

    ContentPage(
        VerticalStackLayout(
            match model.AnimalName with
            | Ok (Animal validName) -> [ makeEntryCell validName ]
            | Error errorMsg -> makeErrorMsg errorMsg
        )
    )

let init () = { AnimalName = validateAnimal "Emu" }
```

Note that the same validation logic can be used in both your app and a service back-end.

### Saving Application State

Application state is very simple to save by serializing the model into `app.Properties`. For example, you can store as JSON as follows using `Json.NET`:

```fsharp

type Application() =
    ....
    let modelId = "model"
    override __.OnSleep() =

        let json = Newtonsoft.Json.JsonConvert.SerializeObject(runner.CurrentModel)
        Debug.WriteLine("OnSleep: saving model into app.Properties, json = {0}", json)

        app.Properties.[modelId] <- json

    override __.OnResume() =
        Debug.WriteLine "OnResume: checking for model in app.Properties"
        try
            match app.Properties.TryGetValue modelId with
            | true, (:? string as json) ->

                Debug.WriteLine("OnResume: restoring model from app.Properties, json = {0}", json)
                let model = Newtonsoft.Json.JsonConvert.DeserializeObject<App.Model>(json)

                Debug.WriteLine("OnResume: restoring model from app.Properties, model = {0}", (sprintf "%0A" model))
                runner.SetCurrentModel (model, Cmd.none)

            | _ -> ()
        with ex ->
            App.program.onError("Error while restoring model found in app.Properties", ex)

    override this.OnStart() =
        Debug.WriteLine "OnStart: using same logic as OnResume()"
        this.OnResume()
```
