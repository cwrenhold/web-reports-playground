#!/bin/bash

# Copy wasm_exec.js to the public/dist folder
cp $(go env GOROOT)/misc/wasm/wasm_exec.js ./FrontendGo/public/dist

# Copy Tiny Go tiny_wasm_exec.js to the public/dist folder
cp $(tinygo env TINYGOROOT)/targets/wasm_exec.js ./FrontendGo/public/dist/tiny_wasm_exec.js
