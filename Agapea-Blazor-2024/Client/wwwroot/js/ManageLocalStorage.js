//Fichero Javascript que define un objeto que cuelga directamente del objeto window (para hacerlo global a todos los comp blazor)
//Con props y metodos para gestionar datos usando el LocalStorage del navegador

 window.adminLocalStorage = {

    almacenarValor: function(clave, valor) {
        localStorage.setItem(clave, JSON.stringify(valor));
    },
    recuperarValor: function(clave){
        return JSON.parse(localStorage.getItem(clave));
    },
         
}