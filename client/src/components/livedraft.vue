<script setup lang="ts">
import type { LeagueDto, PlayerDto, SignPlayerRequest } from '@/api'
import { getPlayers as apiGetPlayers, signPlayer as apiSignPlayer } from '@/api'
import { useManagerStore } from '@/store'
import * as signalR from '@microsoft/signalr'
import { ref } from 'vue'

const props = defineProps<{ league: LeagueDto }>()

const connection = new signalR.HubConnectionBuilder()
  .withUrl('http://localhost:5001/chatHub')
  // .configureLogging()//signalR.LogLevel.Information)
  .build()
const store = useManagerStore()
const players = ref<PlayerDto[]>()

connection.on('signedPlayer', async (teamId: string, playerId: string) => {
  const player = players.value!.find(x => x.id === playerId)!
  player.isSigned = true
  player.teamId = teamId
})

async function loadPlayers() {
  const res = await apiGetPlayers()
  players.value = res
}

let retries = 0
async function start() {
  try {
    await connection.start()
    // eslint-disable-next-line no-console
    console.log('SignalR Connected.')
  }
  catch (err) {
    // eslint-disable-next-line no-console
    console.log(err)
    if (retries < 3) {
      setTimeout(start, 5000)
      retries += 1
    }
    else {
      console.error('Timeout')
    }
  }
};

connection.onclose(async () => {
  await start()
})

async function signPlayer(playerId: string, teamId: string) {
  await apiSignPlayer({ playerId, teamId } as SignPlayerRequest, store.token!)

  //   connection.send('signedPlayer', playerId)

  players.value!.find(x => x.id === playerId)!.isSigned = true
}

start()
</script>

<template>
  <button @click="loadPlayers">
    Ladda om spelare
  </button>
  <div class="grid">
    <template v-for="p in players" :key="p.id">
      <div>{{ p.name }}</div>
      <div>{{ p.position }}</div>
      <div>{{ p.country }}</div>
      <div>{{ (p.isSigned ? `signad av ${league.teams.find(x => x.id === p.teamId)?.name}` : "friii") }}</div>
      <button @click="signPlayer(p.id, props.league.teams[0].id)">
        Signa
      </button>
    </template>
  </div>
</template>

<style scoped>
.grid {
  background-color: #2f2f2f;
  padding: 1rem;
  display: grid;
  gap: 1rem;
  grid-template-columns: repeat(5, 1fr);
  grid-template-rows: auto;
  width: 60rem;
  text-align: left;
}
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
