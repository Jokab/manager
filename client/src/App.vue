<script setup lang="ts">
import * as signalR from "@microsoft/signalr";
import { ref } from "vue";

const connection = new signalR.HubConnectionBuilder()
  .withUrl("http://localhost:5001/chatHub")
  //.configureLogging()//signalR.LogLevel.Information)
  .build();

const message = ref<string>();
const chat = ref<string>("");

connection.on("messageReceived", (user, message) => {
  chat.value = `${chat.value}\n${user}: ${message}`;
})
function send() {
  connection.send("SendMessage", "Jakob", message.value);
  message.value = "";
}

let retries = 0;
async function start() {
  try {
    await connection.start();
    console.log("SignalR Connected.");
  } catch (err) {
    console.log(err);
    if (retries < 3) {
      setTimeout(start, 5000);
      retries += 1;
    } else {
      console.error("Timeout")
    }
  }
};

connection.onclose(async () => {
  await start();
});

start();
</script>

<template>
  <div style="display: flex; flex-direction: column; width: 20rem">
    <textarea
      v-model="chat"
      style="height: 20rem"
    />
    <div>
      <label for="text">Meddelande: </label>
      <input
        id="text"
        v-model="message"
        type="text"
        @keyup.enter="send"
      >
      <button @click="send">
        Hej
      </button>
    </div>
  </div>
</template>

<style scoped>
.logo {
  height: 6em;
  padding: 1.5em;
  will-change: filter;
  transition: filter 300ms;
}

.logo:hover {
  filter: drop-shadow(0 0 2em #646cffaa);
}

.logo.vue:hover {
  filter: drop-shadow(0 0 2em #42b883aa);
}
</style>
