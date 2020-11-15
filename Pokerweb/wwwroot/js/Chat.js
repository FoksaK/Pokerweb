"use strict";


function Hide() {
    var x = document.getElementsByClassName("Playbuttons")
    var i;
    for (i = 0; i < x.length; i++) {
        x[i].style.display = "none";
    }
}

window.onload = function () {

    var isPlaying = "false";

    var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

    connection.on("ShowPlayButton", function () {
        document.getElementById("PlayButton").style.display = "inline";
    });

    connection.on("ReceiveMessage", function () {
        $(function () {
            $('#grid').load('/GamePage/PlayersPartial?key=' +
                document.getElementById("key").innerHTML +
                "&name=" + 
                document.getElementById("name").innerHTML)
        })
    });

    connection.on("ReceivePlayMessage", function () {
        var x = document.getElementsByClassName("Playbuttons")
        var i;
        for (i = 0; i < x.length; i++) {
            x[i].style.display = "inline";
        }
        isPlaying = "true";
    });

    connection.start().then(function () {
        connection.invoke("Connected", document.getElementById("key").innerHTML, document.getElementById("name").innerHTML).catch(function (err) {
            return console.error(err.toString());
            });
        }).catch(function (err) {
            return console.error(err.toString());
    });


    document.getElementById("Fold").addEventListener("click", function (event) {
        Hide();
        isPlaying = "false";
        connection.invoke("FoldMessage", document.getElementById("key").innerHTML,
            document.getElementById("name").innerHTML)
            .catch(function (err) {return console.error(err.toString());
            });
        
        event.preventDefault();
    });

    document.getElementById("Check").addEventListener("click", function (event) {
        Hide();
        isPlaying = "false";
        connection.invoke("CheckMessage", document.getElementById("key").innerHTML,
            document.getElementById("name").innerHTML)
            .catch(function (err) {
                return console.error(err.toString());
            });
        event.preventDefault();
    });

    document.getElementById("Raise").addEventListener("click", function (event) {
        Hide();
        isPlaying = "false";
        connection.invoke("RaiseMessage", document.getElementById("key").innerHTML,
            document.getElementById("name").innerHTML, document.getElementById("demo").innerHTML)//problém se sliderem
            .catch(function (err) {
                return console.error(err.toString());
            });
        event.preventDefault();
    });

    document.getElementById("PlayButton").addEventListener("click", function (event) {
        connection.invoke("StartMessage", document.getElementById("key").innerHTML)
            .catch(function (err) {
                return console.error(err.toString());
            });
        event.preventDefault();

        document.getElementById("PlayButton").style.display = "none";
    });

    window.onunload = function () {
        connection.invoke("LeaveMessage", document.getElementById("key").innerHTML,
            document.getElementById("name").innerHTML, isPlaying)
    };
 
};