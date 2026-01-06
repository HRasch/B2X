<template>
  <div class="logs-view">
    <h2>Client Logs</h2>
    <div>
      <label>Filter: </label>
      <input v-model="q" @keyup.enter="load" placeholder="search message/user/route" />
      <button @click="load">Reload</button>
    </div>
    <table>
      <thead>
        <tr>
          <th>Time</th>
          <th>Level</th>
          <th>Tenant</th>
          <th>User</th>
          <th>Route</th>
          <th>Message</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="entry in logs" :key="entry.id">
          <td>{{ entry.timestamp }}</td>
          <td>{{ entry.level }}</td>
          <td>{{ entry.tenantId }}</td>
          <td>{{ entry.userId }}</td>
          <td>{{ entry.route }}</td>
          <td>
            <pre>{{ entry.message }}</pre>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script lang="ts">
import { defineComponent, ref } from 'vue';
import axios from 'axios';

export default defineComponent({
  name: 'LogsView',
  setup() {
    const logs = ref<Array<any>>([]);
    const q = ref('');

    async function load() {
      const res = await axios.get('/api/logs/errors', { params: { q: q.value } });
      logs.value = res.data || [];
    }

    load();
    return { logs, q, load };
  },
});
</script>

<style scoped>
table {
  width: 100%;
  border-collapse: collapse;
}
th,
td {
  border: 1px solid #ddd;
  padding: 8px;
  text-align: left;
}
pre {
  margin: 0;
  white-space: pre-wrap;
}
</style>
