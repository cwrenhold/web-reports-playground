package main

import (
	"strings"
	"syscall/js"
)

func concatenate(arr []string) string {
	return strings.Join(arr, ",")
}

func main() {
	js.Global().Set("processData", js.FuncOf(func(this js.Value, args []js.Value) interface{} {
		jsArray := args[0]
		goArray := make([]string, jsArray.Length())

		for i := 0; i < jsArray.Length(); i++ {
			goArray[i] = jsArray.Index(i).String()
		}

		result := concatenate(goArray)

		// Create a JavaScript object to return
		jsObject := js.Global().Get("Object").New()
		jsObject.Set("yourResult", result)
		return jsObject
	}))
	select {}
}
