#!/bin/bash

# Copy wasm_exec.js to the public/dist folder
cp $(go env GOROOT)/misc/wasm/wasm_exec.js ./FrontendGo/public/dist
