<template>
  <footer class="footer">
    <div class="footer-content">
      <div class="footer-section">
        <h3>{{ t("common.about") }}</h3>
        <ul>
          <li>
            <RouterLink to="/">{{ t("common.home") }}</RouterLink>
          </li>
          <li>
            <a href="#impressum">{{ t("menu.impressum") }}</a>
          </li>
          <li>
            <a href="#privacy">{{ t("menu.privacy") }}</a>
          </li>
        </ul>
      </div>

      <div class="footer-section">
        <h3>{{ t("common.help") }}</h3>
        <ul>
          <li>
            <RouterLink to="/faq">{{ t("menu.faq") }}</RouterLink>
          </li>
          <li>
            <RouterLink to="/contact">{{ t("menu.contact") }}</RouterLink>
          </li>
          <li>
            <a href="mailto:support@example.com">{{
              t("menu.email_support")
            }}</a>
          </li>
        </ul>
      </div>

      <div class="footer-section">
        <h3>{{ t("common.help") }}</h3>
        <ul>
          <li>
            <RouterLink to="/dispute-resolution">
              {{ t("disputes.footer_link") }}
            </RouterLink>
          </li>
          <li>
            <a
              href="https://ec.europa.eu/consumers/odr/"
              target="_blank"
              rel="noopener noreferrer"
            >
              {{ t("disputes.eu_odr_title") }}
            </a>
          </li>
        </ul>
      </div>

      <div class="footer-section">
        <h3>{{ t("common.language") }}</h3>
        <div class="language-selector">
          <LanguageSwitcher />
        </div>
      </div>
    </div>

    <div class="footer-bottom">
      <p>&copy; 2025 B2Connect. {{ t("common.version") }} 1.0</p>
    </div>
  </footer>
</template>

<script setup lang="ts">
import { useI18n } from "vue-i18n";
import { RouterLink } from "vue-router";
import LanguageSwitcher from "./LanguageSwitcher.vue";

const { t } = useI18n();
</script>

<style scoped>
.footer {
  background-color: #2c3e50;
  color: #ecf0f1;
  padding: 3rem 0 1rem;
  margin-top: 4rem;
  border-top: 1px solid #34495e;
}

.footer-content {
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 2rem;
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 2rem;
  margin-bottom: 2rem;
}

.footer-section h3 {
  color: #3498db;
  margin-bottom: 1rem;
  font-size: 1.1rem;
}

.footer-section ul {
  list-style: none;
  padding: 0;
  margin: 0;
}

.footer-section li {
  margin-bottom: 0.75rem;
}

.footer-section a,
.footer-section :deep(a) {
  color: #ecf0f1;
  text-decoration: none;
  transition: color 0.3s ease;
}

.footer-section a:hover,
.footer-section :deep(a:hover) {
  color: #3498db;
}

.language-selector {
  margin-top: 0.5rem;
}

.footer-bottom {
  text-align: center;
  padding-top: 2rem;
  border-top: 1px solid #34495e;
  color: #95a5a6;
  font-size: 0.9rem;
}

@media (max-width: 768px) {
  .footer-content {
    grid-template-columns: 1fr;
    gap: 1.5rem;
  }

  .footer {
    padding: 2rem 0 1rem;
    margin-top: 2rem;
  }
}
</style>
