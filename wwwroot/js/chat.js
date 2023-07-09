"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("").build();

connection.start().then(function () {
    console.log("Connect success")
}).catch(function (err) {
    return console.error(err.toString());
});
//var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

//Disable the send button until connection is established.
document.getElementsByClassName("btn").disabled = true;

connection.on("ReceiveMessage", function (user, message) {
    var li = document.createElement("li");
    document.getElementById("messagesList").appendChild(li);
    li.textContent = `${user} send: ${message}`;
});
connection.on("UpdateRemainingTime", function (message) {
    var li = document.createElement("li");
    document.getElementById("messagesList").appendChild(li);
    // We can assign user-supplied strings to an element's textContent because it
    // is not interpreted as markup. If you're assigning in any other way, you 
    // should be aware of possible script injection concerns.
    li.textContent = `Time: ${message}`;
});
connection.on("CancelSuccess", function () {
    var li = document.createElement("li");
    document.getElementById("messagesList").appendChild(li);
    li.textContent = `Message: ${message}`;
});

document.getElementById("submitRequest").addEventListener("click", function (event) {
    var user = document.getElementById("userid").value;
    var message = document.getElementById("number").value;
    connection.invoke("OnReceiveMatchMakingRequest", user, message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

document.getElementById("Test").addEventListener("click", function (event) {

    var message = document.getElementById("message").value;
    connection.invoke("SendMessage", "Tester", message.toString).catch(function (err) {
        console.log("Lỗi")
        return console.error(err.toString());
    });
    event.preventDefault();
});