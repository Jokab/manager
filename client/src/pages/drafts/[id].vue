<script setup lang="ts">
import type { DraftDto, LeagueDto } from '@/api'
import { getDraft as apiGetDraft, startDraft as apiStartDraft, getLeague } from '@/api'
import Livedraft from '@/components/livedraft.vue'
import { useManagerStore } from '@/store'

const store = useManagerStore()
const { params } = useRoute('/drafts/[id]')

const draft = ref<DraftDto>()
const league = ref<LeagueDto>()

draft.value = await apiGetDraft(params.id, store.token!)
league.value = await getLeague(draft.value.leagueId, store.token!)

async function startDraft() {
  const res = await apiStartDraft({ draftId: draft.value!.id }, store.token!)
  draft.value!.state = res.state
}
</script>

<template>
  <div style="display: flex; flex-direction: column">
    <div>hej draft! id är: {{ draft?.id }} med status {{ draft?.state }}</div>
    <div>
      medverkande lag:
      <ul>
        <li
          v-for="team in league?.teams" :key="team.id"
        >
          {{ team.name }}
        </li>
      </ul>
    </div>
    <button @click="startDraft">
      Påbörja draft!
    </button>
    <Livedraft :league="league!" />
  </div>
</template>
