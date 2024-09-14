const connection = new signalR.HubConnectionBuilder().withUrl("/presence").build();
connection.start()
    .then(() => {
        console.log("SignalR connection established.");
        // Connection is successfully established, you can now send messages
    })
    .catch((error) => {
        console.error("Error establishing SignalR connection:", error);
    });

connection.on("UserIsOnline", function (name) {
    console.log(name)
});
connection.on("UserIsOffline", function (name) {
    console.log(name)
});
connection.on("NewMessageReceived", function (obj) {
    showToast("success", obj.username + " has sent you message");
    playMessageSound();
});

function playMessageSound() {
    const sound = document.getElementById("messageSound");
    sound.play();
}