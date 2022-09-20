"use strict";
var userId = '@userId';
var connection = new signalR.HubConnectionBuilder().withUrl("/chat").build();

//Disable the send button until connection is established.
document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (data) {
    console.log(data);
    var classForMessage = ((data.userId == userId) ? 'my-message' : 'other-message float-right');
    console.log(classForMessage);

    var message = `
                <li class="clearfix">
                            <div class="message ${classForMessage}">${data.message}</div>
                    </li>
        `;
    //var li = document.createElement("li");
    $("#chat-messages").append(message);
    //// We can assign user-supplied strings to an element's textContent because it
    //// is not interpreted as markup. If you're assigning in any other way, you
    //// should be aware of possible script injection concerns.
    //li.textContent = `${data.UserId} says ${data.Message}`;
    console.log(message);
});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("textMessage")
    .addEventListener("keyup", function (event) {
        event.preventDefault();
        if (event.keyCode === 13) {
            document.getElementById("sendButton").click();
        }
    });

document.getElementById("sendButton").addEventListener("click", function (event) {
    var message = document.getElementById("textMessage").value;
    var data = {
        UserId: '@userId',
        ChatId: '@chat.Id',
        Message: message,
    }
    connection.invoke("SendMessage", data).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
    document.getElementById("textMessage").value = '';
});