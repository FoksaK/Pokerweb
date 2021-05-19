"use strict";


function Hide() {
    var x = document.getElementsByClassName("Playbuttons")
    var i;
    for (i = 0; i < x.length; i++) {
        x[i].style.display = "none";
    }
}

var loaded = false;

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
        add();
    });

    connection.on("ReceivePlayMessage", function () {
        var x = document.getElementsByClassName("Playbuttons")
        var i;
        for (i = 0; i < x.length; i++) {
            x[i].style.display = "inline";
        }
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

    function Next() {

        var x = document.getElementsByClassName("pContainer");
        var a = document.getElementById("actual");
        var number = 0;

        var z;

        if (actualCard == null) {
            number = a.innerHTML + 1;
        } else {
            number = actualCard + 1;
        }

        for (z = 0; z < x.length; z++) {
            x[z].style.display = "none"
        }

        if (number >= x.length) {
            number = 0;
        }
        if (number <= x.length && number >= 0) {
            x[number].style.display = "block";
            container.classList.remove("slide");
            container.classList.remove("slideReversed");
            void container.offsetWidth;
            container.classList.add("slide");
        }

        actualCard = number;
        
    }

    function Previous() {
        var container = document.getElementById("container");
        var x = document.getElementsByClassName("pContainer");
        var a = document.getElementById("actual");
        var number = 0;

        var z;

        if (actualCard == null) {
            number = a.innerHTML - 1;
        } else {
            number = actualCard - 1;
        }

        for (z = 0; z < x.length; z++) {
            x[z].style.display = "none"
        }

        if (number < 0) {
            number = x.length - 1;
        }
        if (number <= x.length && number >= 0) {
            x[number].style.display = "block";
            container.classList.remove("slideReversed");
            container.classList.remove("slide");
            void container.offsetWidth;
            container.classList.add("slideReversed");
        }

        actualCard = number;

        
    }

    var actualCard = null;
    var x = document.getElementById("nextCardBtn");

    var y = document.getElementById("previousCardBtn");

    function add() {
        y.onclick = function () { Previous() };
        x.onclick = function () { Next() };
    }
};
