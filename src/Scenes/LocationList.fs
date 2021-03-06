module LocationList

open System
open Fable.Core
open Fable.Import
open Fable.Import.ReactNative
open Fable.Import.ReactNativeImagePicker
open Fable.Helpers.ReactNative
open Fable.Helpers.ReactNative.Props
open Fable.Core.JsInterop
open Elmish

// Model
type Status =
| NotStarted
| InProgress
| Complete of string

type Msg =
| CheckNextLocation of int * Model.LocationCheckRequest
| GoBack
| RefreshList
| NewLocationCheckRequests of (int * Model.LocationCheckRequest)[]
| Error of exn

type Model =
  { RequestDataSource : ListViewDataSource<int * Model.LocationCheckRequest>
    Status : Status }

let init () =
    { Status = NotStarted
      RequestDataSource = emptyDataSource() }, Cmd.ofMsg RefreshList

// Update
let update msg model : Model*Cmd<Msg> =
    match msg with
    | GoBack
    | CheckNextLocation _ ->
        model, Cmd.none // Handled one level above

    | RefreshList ->
        { model with Status = InProgress },
        Cmd.ofPromise Database.getIndexedCheckRequests () NewLocationCheckRequests Error

    | NewLocationCheckRequests indexedRequests ->
        { model with
            RequestDataSource = updateDataSource indexedRequests model.RequestDataSource
            Status = Complete (sprintf "Locations: %d" indexedRequests.Length) }, Cmd.none

    | Error e ->
        { model with
            Status = Complete e.Message }, Cmd.none

// View
let view (model:Model) (dispatch: Msg -> unit) =
    let getListView() =
          listView
            model.RequestDataSource
            [ ListViewProperties.RenderRow
                (Func<_,_,_,_,_>(fun (pos,request: Model.LocationCheckRequest) b c d ->
                    [view [
                        ViewProperties.Style[
                            FlexStyle.JustifyContent JustifyContent.Center
                            FlexStyle.AlignItems ItemAlignment.Center
                            FlexStyle.Flex 1.
                            FlexStyle.FlexDirection FlexDirection.Row ]]
                        [ text [ Styles.defaultText ] request.Name
                          text [ Styles.defaultText ] request.Address
                          (match request.Status with
                           | None -> text [] ""
                           | Some status ->
                                let uri =
                                    match status with
                                    | Model.LocationStatus.Alarm text -> localImage "${entryDir}/../images/Alarm.png"
                                    | _ -> localImage "${entryDir}/../images/Approve.png"

                                image
                                    [ Source uri
                                      ImageProperties.Style [
                                            FlexStyle.Width 24.
                                            FlexStyle.Height 24.
                                            FlexStyle.AlignSelf Alignment.Center
                                        ]
                                    ])
                    ]]
                    |> touchableHighlight [OnPress (fun () -> dispatch (CheckNextLocation(pos,request)))]))
            ]

    view [ Styles.sceneBackground ]
        [ text [ Styles.titleText ] "Locations to check"
          text [ Styles.defaultText ]
            (match model.Status with
             | Complete s -> s
             | _ -> "")
          (if model.RequestDataSource.getRowCount() > 0 then getListView() else Styles.whitespace)

          Styles.button "OK" (fun () -> dispatch GoBack)
        ]