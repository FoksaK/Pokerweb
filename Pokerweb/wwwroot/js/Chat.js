"use strict";

window.onload = function () {

    var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

    //Disable send button until connection is established


    connection.on("ReceiveMessage", function () {
        $(function () {
            $('#grid').load('/GamePage?handler=PlayersPartial');
        });
    });

    connection.start().then(function () {
        document.getElementById("sendButton").disabled = false;
    }).catch(function (err) {
        return console.error(err.toString());
    });

    document.getElementById("sendButton").addEventListener("click", function (event) {
        connection.invoke("SendMessage", 12).catch(function (err) {
            return console.error(err.toString());
        });
        event.preventDefault();
    });

    

    document.getElementById("Fold").addEventListener("click", function (event) {
        connection.invoke("SendMessage", 2).catch(function (err) {
            return console.error(err.toString());
        });
        event.preventDefault();
    });
};