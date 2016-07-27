namespace Fable.Import

open System
open Fable.Core
open Fable.Import
open Fable.Import.JS
open Fable.Import.Browser

module ReactImagePicker =

    type ImagePicker =
        abstract member showImagePicker: ImagePickerOptions * (ImagePickerResult -> unit) -> unit
        abstract member launchCamera: ImagePickerOptions * (ImagePickerResult -> unit) -> unit
        abstract member launchImageLibrary: ImagePickerOptions * (ImagePickerResult -> unit) -> unit

    and ImagePickerOptions =
        abstract title: string option with get, set
        abstract cancelButtonTitle: string option with get, set
        abstract takePhotoButtonTitle: string option with get, set
        abstract chooseFromLibraryButtonTitle: string option with get, set
        abstract allowsEditing: bool option with get, set

    and ImagePickerResult =     
        abstract didCancel: bool with get, set
        abstract error: string with get, set
        abstract uri: string with get, set

    type Globals =
        [<Import("default", from="react-native-image-picker")>]
        static member ImagePicker with get(): ImagePicker = failwith "JS only" and set(v: ImagePicker): unit = failwith "JS only"