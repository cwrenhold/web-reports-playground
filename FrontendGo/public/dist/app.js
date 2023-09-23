const go = new Go();

async function loadWasm() {
    const result = await WebAssembly.instantiateStreaming(fetch("dist/main.wasm"), go.importObject);
    go.run(result.instance);
}

async function processWithWasm() {
    const data = { yourArray: ["one", "two", "three"] };//await fetchData();
    const processedObject = window.processData(data.yourArray); // assuming the fetched object has a key 'yourArray' that's an array of strings
    console.log(processedObject);
}

loadWasm().then(() => {
    processWithWasm();
});
