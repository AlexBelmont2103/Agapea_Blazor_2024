//Fichero Javascript que define un objeto que cuelga directamente del objeto window (para hacerlo global a todos los comp blazor)
//Con props y metodos para gestionar datos usando Indexed DB del navegador
//Como las operaciones son asincronas, cuando finalice la operacion sobre la api del navegador, debemos llamar a metodos de Blazor para que actualice el estado de la app
//Pasamos una referencia del servicio,compononente a los metodos js desde los que te interese
//Invoca a metodos de Blazor, y con esa referencia invocar dichos metodos
//Esta referencia se define en el servicio o componente objeto clase: DotNetObjectReference.Create(this)

function _crearEsquemaIndexedDB() {
    //Crear esquema de la base de datos
    var request = window.indexedDB.open("AgapeaDB", 1);
    request.onupgradeneeded = function (event) {
        var db = event.target.result;
        var clientesObjectStore = db.createObjectStore("datosclientes", { keyPath: "email" }); //Objeto IDBObjectStore
        clientesObjectStore.createIndex("email", "email", { unique: true });

        var tokensObjectStore = db.createObjectStore("tokens", { keyPath: "email" });
        tokensObjectStore.createIndex("email","email", {unique: true})
        
    };
    request.onsuccess = function (event) {
        console.log("Base de datos creada correctamente");
    };
    request.onerror = function (event) {
        console.log("Error creando la base de datos");
    };
}
window.adminIndexedDB = {
    recuperarDatosCliente(refServicioNET, email) {
        var _reqDB = indexedDB.open("AgapeaDB", 1);
        _reqDB.addEventListener("upgradeneeded", _crearEsquemaIndexedDB);
        _reqDB.addEventListener("success", (ev) => {
            //Para recuperar datos asociados a un cliente, del ObjectStore datosclientes
            //Creo un objeto de tipo transaction readonly
            var _db = ev.target.result;
            var _txDatosCliente = _db.transaction(['datosclientes'], 'readonly');
            var _selectDatosCliente = _txDatosCliente.objectStore('datosclientes').get(email);
                _selectDatosCliente.addEventListener("success", (ev) => {
                    var _datosCliente = ev.target.result;
                    //Invocar metodo de Blazor para que actualice el estado de la app
                    refServicioNET.invokeMethodAsync('CallbackServicioIndexedDBblazor', _datosCliente);
                });
                _selectDatosCliente.addEventListener("error", (ev) => {
                    console.log("Error recuperando datos del cliente");
                });
        });
        _reqDB.addEventListener("error", (ev) => {
            console.log("Error recuperando datos del cliente");
        });
    },
    almacenarDatosCliente(datosCliente) {
        var _reqDB = indexedDB.open("AgapeaDB", 1);
        _reqDB.addEventListener("upgradeneeded", _crearEsquemaIndexedDB);
        _reqDB.addEventListener("success", (ev) => {
            //Para guardar datos asociados a un cliente, en el ObjectStore datosclientes
            //Creo un objeto de tipo transaction readwrite
            var _db = ev.target.result;
            var _txDatosCliente = _db.transaction(['datosclientes'], 'readwrite');
            var _insertDatosCliente = _txDatosCliente.objectStore('datosclientes').add(datosCliente, datosCliente.credenciales.email);
        });
        _reqDB.addEventListener("error", (ev) => {
            console.log("Error guardando datos del cliente");
        });
    },
    recueprarTokenCliente(refServicioNET, email) {
        var _reqDB = indexedDB.open("AgapeaDB", 1);
        _reqDB.addEventListener("upgradeneeded", _crearEsquemaIndexedDB);
        _reqDB.addEventListener("success", (ev) => {
            //Para recuperar datos asociados a un cliente, del ObjectStore tokens
            //Creo un objeto de tipo transaction readonly
            var _db = ev.target.result;
            var _txTokens = _db.transaction(['tokens'], 'readonly');
            var _selectToken = _txTokens.objectStore('tokens').get(email);
            _selectToken.addEventListener("success", (ev) => {
                var _token = ev.target.result;
                //Invocar metodo de Blazor para que actualice el estado de la app
                //Habría que sobrecargar el metodo???
                refServicioNET.invokeMethodAsync('CallbackServicioIndexedDBblazor', _token);
            });
            _selectToken.addEventListener("error", (ev) => {
                console.log("Error recuperando token del cliente");
            });
        });
        _reqDB.addEventListener("error", (ev) => {
            console.log("Error recuperando token del cliente");
        });
    },
    almacenarTokenCliente(token) {
        var _reqDB = indexedDB.open("AgapeaDB", 1);
        _reqDB.addEventListener("upgradeneeded", _crearEsquemaIndexedDB);
        _reqDB.addEventListener("success", (ev) => {
            //Para guardar datos asociados a un cliente, en el ObjectStore tokens
            //Creo un objeto de tipo transaction readwrite
            var _db = ev.target.result;
            var _txToken = _db.transaction(['tokens'], 'readwrite');
            var _insertToken = _txToken.objectStore('tokens').add(token, token.email);
        });
        _reqDB.addEventListener("error", (ev) => {
            console.log("Error guardando token del cliente");
        });
    },
};