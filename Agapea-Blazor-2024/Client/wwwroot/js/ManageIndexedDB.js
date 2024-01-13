//Fichero Javascript que define un objeto que cuelga directamente del objeto window (para hacerlo global a todos los comp blazor)
//Con props y metodos para gestionar datos usando Indexed DB del navegador
 window.adminIndexedDB = {
     almacenarValor: function(clave, valor) {
         var request = window.indexedDB.open("MiBaseDeDatos", 1);

         request.onerror = function(event) {
             console.log("Error al abrir la base de datos");
         };

         request.onsuccess = function(event) {
             var db = event.target.result;
             var transaction = db.transaction(["MiAlmacen"], "readwrite");
             var objectStore = transaction.objectStore("MiAlmacen");
             var request = objectStore.put(valor, clave);

             request.onerror = function(event) {
                 console.log("Error al almacenar el valor");
             };

             request.onsuccess = function(event) {
                 console.log("Valor almacenado correctamente");
             };
         };

         request.onupgradeneeded = function(event) {
             var db = event.target.result;
             var objectStore = db.createObjectStore("MiAlmacen");
         };
     },

     recuperarValor: function(clave) {
         var request = window.indexedDB.open("MiBaseDeDatos", 1);

         request.onerror = function(event) {
             console.log("Error al abrir la base de datos");
         };

         request.onsuccess = function(event) {
             var db = event.target.result;
             var transaction = db.transaction(["MiAlmacen"], "readonly");
             var objectStore = transaction.objectStore("MiAlmacen");
             var request = objectStore.get(clave);

             request.onerror = function(event) {
                 console.log("Error al recuperar el valor");
             };

             request.onsuccess = function(event) {
                 var valor = event.target.result;
                 console.log("Valor recuperado correctamente:", valor);
                 return valor;
             };
         };

         request.onupgradeneeded = function(event) {
             var db = event.target.result;
             var objectStore = db.createObjectStore("MiAlmacen");
         };
     }
 };