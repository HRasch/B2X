<template>
  <div class="ai-assistant">
    <!-- AI Assistant Toggle Button -->
    <button
      @click="toggleAssistant"
      class="ai-toggle-btn"
      :class="{ 'active': isVisible }"
      aria-label="Toggle AI Assistant"
    >
      <svg class="ai-icon" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
        <path d="M12 2C13.1 2 14 2.9 14 4V5H16C17.1 5 18 5.9 18 7V19C18 20.1 17.1 21 16 21H8C6.9 21 6 20.1 6 19V7C6 5.9 6.9 5 8 5H10V4C10 2.9 10.9 2 12 2ZM12 4V5H12V4ZM8 7V19H16V7H8ZM10 9H14V11H10V9ZM10 13H14V15H10V13Z" fill="currentColor"/>
      </svg>
      <span class="ai-label">AI Assistant</span>
    </button>

    <!-- AI Assistant Panel -->
    <div v-if="isVisible" class="ai-panel" :class="{ 'minimized': isMinimized }">
      <!-- Panel Header -->
      <div class="ai-header">
        <div class="ai-title">
          <svg class="ai-brain-icon" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
            <path d="M12 2C13.1 2 14 2.9 14 4V5H16C17.1 5 18 5.9 18 7V19C18 20.1 17.1 21 16 21H8C6.9 21 6 20.1 6 19V7C6 5.9 6.9 5 8 5H10V4C10 2.9 10.9 2 12 2ZM12 4V5H12V4ZM8 7V19H16V7H8Z" fill="currentColor"/>
          </svg>
          <span>Management AI Assistant</span>
        </div>
        <div class="ai-controls">
          <button @click="isMinimized = !isMinimized" class="control-btn" aria-label="Minimize">
            <svg viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
              <path d="M19 13H5V11H19V13Z" fill="currentColor"/>
            </svg>
          </button>
          <button @click="closeAssistant" class="control-btn" aria-label="Close">
            <svg viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
              <path d="M19 6.41L17.59 5 12 10.59 6.41 5 5 6.41 10.59 12 5 17.59 6.41 19 12 13.41 17.59 19 19 17.59 13.41 12 19 6.41Z" fill="currentColor"/>
            </svg>
          </button>
        </div>
      </div>

      <!-- Panel Content -->
      <div v-if="!isMinimized" class="ai-content">
        <!-- Quick Actions -->
        <div class="quick-actions">
          <h4>Quick Actions</h4>
          <div class="action-buttons">
            <button @click="quickAction('analyze-performance')" class="action-btn">
              <svg viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                <path d="M3 13H7V11H3V13ZM3 17H7V15H3V17ZM3 9H7V7H3V9ZM13 13H21V11H13V13ZM13 17H21V15H13V17ZM13 9H21V7H13V9Z" fill="currentColor"/>
              </svg>
              Analyze Performance
            </button>
            <button @click="quickAction('security-audit')" class="action-btn">
              <svg viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                <path d="M18 8H17V6C17 3.24 14.76 1 12 1S7 3.24 7 6V8H6C4.9 8 4 8.9 4 10V20C4 21.1 4.9 22 6 22H18C19.1 22 20 21.1 20 20V10C20 8.9 19.1 8 18 8ZM12 17C10.9 17 10 16.1 10 15S10.9 13 12 13 14 13.9 14 15 13.1 17 12 17ZM9 8V6C9 4.34 10.34 3 12 3S15 4.34 15 6V8H9Z" fill="currentColor"/>
              </svg>
              Security Audit
            </button>
            <button @click="quickAction('optimize-content')" class="action-btn">
              <svg viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                <path d="M14 2H6C4.9 2 4 2.9 4 4V20C4 21.1 4.9 22 6 22H18C19.1 22 20 21.1 20 20V8L14 2ZM18 20H6V4H13V9H18V20ZM9 13H7V11H9V13ZM9 17H7V15H9V17ZM9 9H7V7H9V9Z" fill="currentColor"/>
              </svg>
              Content Optimization
            </button>
          </div>
        </div>

        <!-- Chat Interface -->
        <div class="chat-container">
          <div class="messages" ref="messagesContainer">
            <div
              v-for="message in messages"
              :key="message.id"
              class="message"
              :class="{ 'user': message.sender === 'user', 'assistant': message.sender === 'assistant' }"
            >
              <div class="message-content">
                <div class="message-text" v-html="formatMessage(message.text)"></div>
                <div class="message-time">{{ formatTime(message.timestamp) }}</div>
              </div>
            </div>
            <div v-if="isTyping" class="message assistant typing">
              <div class="message-content">
                <div class="typing-indicator">
                  <span></span>
                  <span></span>
                  <span></span>
                </div>
              </div>
            </div>
          </div>

          <!-- Message Input -->
          <div class="message-input">
            <textarea
              v-model="currentMessage"
              @keydown.enter.exact.prevent="sendMessage"
              @keydown.enter.shift.exact="currentMessage += '\n'"
              placeholder="Ask me anything about your management tasks..."
              class="input-field"
              rows="1"
              ref="messageInput"
            ></textarea>
            <button
              @click="sendMessage"
              :disabled="!currentMessage.trim() || isTyping"
              class="send-btn"
            >
              <svg viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                <path d="M2 21L23 12L2 3V10L17 12L2 14V21Z" fill="currentColor"/>
              </svg>
            </button>
          </div>
        </div>

        <!-- Context Awareness -->
        <div class="context-info">
          <div class="context-item">
            <span class="context-label">Current Page:</span>
            <span class="context-value">{{ currentPage }}</span>
          </div>
          <div class="context-item">
            <span class="context-label">Tenant:</span>
            <span class="context-value">{{ currentTenant }}</span>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, nextTick, onMounted, onUnmounted, watch } from 'vue'
import { marked } from 'marked'
import aiService from '@/services/aiService'

// Props
interface Props {
  tenantId?: string
}

const props = withDefaults(defineProps<Props>(), {
  tenantId: ''
})

// Reactive state
const isVisible = ref(false)
const isMinimized = ref(false)
const messages = ref<Array<{
  id: string
  sender: 'user' | 'assistant'
  text: string
  timestamp: Date
}>>([])
const currentMessage = ref('')
const isTyping = ref(false)
const currentPage = ref('Dashboard')
const currentTenant = ref('Default Tenant')

// Refs
const messagesContainer = ref<HTMLElement>()
const messageInput = ref<HTMLTextAreaElement>()

// Methods
const toggleAssistant = () => {
  isVisible.value = !isVisible.value
  if (isVisible.value) {
    isMinimized.value = false
    nextTick(() => {
      messageInput.value?.focus()
    })
  }
}

const closeAssistant = () => {
  isVisible.value = false
  isMinimized.value = false
}

const quickAction = async (action: string) => {
  let message = ''

  switch (action) {
    case 'analyze-performance':
      message = 'Can you analyze the current system performance and provide optimization recommendations?'
      break
    case 'security-audit':
      message = 'Please conduct a security audit of our current setup and provide compliance recommendations.'
      break
    case 'optimize-content':
      message = 'Help me optimize the content on this page for better SEO and user engagement.'
      break
  }

  if (message) {
    currentMessage.value = message
    await sendMessage()
  }
}

const sendMessage = async () => {
  if (!currentMessage.value.trim() || isTyping.value) return

  const userMessage = currentMessage.value.trim()
  currentMessage.value = ''

  // Add user message
  messages.value.push({
    id: Date.now().toString(),
    sender: 'user',
    text: userMessage,
    timestamp: new Date()
  })

  // Show typing indicator
  isTyping.value = true

  try {
    // Call MCP server
    const response = await callMcpTool(userMessage)

    // Add assistant response
    messages.value.push({
      id: (Date.now() + 1).toString(),
      sender: 'assistant',
      text: response,
      timestamp: new Date()
    })
  } catch (error) {
    console.error('AI Assistant error:', error)
    messages.value.push({
      id: (Date.now() + 1).toString(),
      sender: 'assistant',
      text: 'Sorry, I encountered an error. Please try again.',
      timestamp: new Date()
    })
  } finally {
    isTyping.value = false
    nextTick(() => {
      scrollToBottom()
    })
  }
}

const callMcpTool = async (message: string): Promise<string> => {
  // Determine which tool to use based on message content
  const toolName = determineTool(message)
  const args = extractArgs(message, toolName)

  try {
    const result = await aiService.callMcpTool({
      name: toolName,
      arguments: args
    })

    return result
  } catch (error) {
    console.error('AI service error:', error)
    throw error
  }
}

const determineTool = (message: string): string => {
  const lowerMessage = message.toLowerCase()

  if (lowerMessage.includes('performance') || lowerMessage.includes('optimize') || lowerMessage.includes('speed')) {
    return 'performance_optimization'
  }
  if (lowerMessage.includes('security') || lowerMessage.includes('audit') || lowerMessage.includes('compliance')) {
    return 'security_compliance'
  }
  if (lowerMessage.includes('content') || lowerMessage.includes('seo') || lowerMessage.includes('engagement')) {
    return 'content_optimization'
  }
  if (lowerMessage.includes('store') || lowerMessage.includes('sales') || lowerMessage.includes('inventory')) {
    return 'store_operations'
  }
  if (lowerMessage.includes('tenant') || lowerMessage.includes('resource') || lowerMessage.includes('onboard')) {
    return 'tenant_management'
  }
  if (lowerMessage.includes('integration') || lowerMessage.includes('api') || lowerMessage.includes('webhook')) {
    return 'integration_management'
  }
  if (lowerMessage.includes('user') || lowerMessage.includes('admin') || lowerMessage.includes('role')) {
    return 'user_management_assistant'
  }
  if (lowerMessage.includes('email') || lowerMessage.includes('template') || lowerMessage.includes('campaign')) {
    return 'email_template_design'
  }
  if (lowerMessage.includes('page') || lowerMessage.includes('layout') || lowerMessage.includes('cms')) {
    return 'cms_page_design'
  }
  if (lowerMessage.includes('health') || lowerMessage.includes('system') || lowerMessage.includes('monitor')) {
    return 'system_health_analysis'
  }

  // Default to content optimization for general queries
  return 'content_optimization'
}

const extractArgs = (message: string, toolName: string): any => {
  // Basic argument extraction - can be enhanced with NLP
  const args: any = {}

  switch (toolName) {
    case 'performance_optimization':
      args.component = extractComponent(message) || 'system'
      args.metricType = 'response_time'
      args.includeHistoricalData = true
      break
    case 'security_compliance':
      args.assessmentType = 'comprehensive'
      args.scope = 'all'
      args.includeRecommendations = true
      break
    case 'content_optimization':
      args.contentType = 'web_page'
      args.content = 'Current page content'
      break
    case 'store_operations':
      args.operation = 'analyze'
      args.storeId = 'current'
      args.analysisType = 'performance'
      break
    case 'tenant_management':
      args.action = 'analyze'
      args.tenantId = props.tenantId || 'current'
      break
    case 'integration_management':
      args.integrationType = 'api'
      args.action = 'analyze'
      break
    case 'user_management_assistant':
      args.task = message
      break
    case 'email_template_design':
      args.emailType = 'marketing'
      args.contentPurpose = 'engagement'
      break
    case 'cms_page_design':
      args.pageType = 'landing'
      args.contentRequirements = 'conversion focused'
      break
    case 'system_health_analysis':
      args.component = 'all'
      args.timeRange = '24h'
      break
  }

  return args
}

const extractComponent = (message: string): string => {
  if (message.includes('database')) return 'database'
  if (message.includes('api')) return 'api'
  if (message.includes('frontend')) return 'frontend'
  if (message.includes('backend')) return 'backend'
  return 'system'
}

const formatMessage = (text: string): string => {
  // Convert markdown to HTML
  return marked(text, { breaks: true })
}

const formatTime = (timestamp: Date): string => {
  return timestamp.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })
}

const scrollToBottom = () => {
  nextTick(() => {
    if (messagesContainer.value) {
      messagesContainer.value.scrollTop = messagesContainer.value.scrollHeight
    }
  })
}

// Auto-resize textarea
const autoResizeTextarea = () => {
  if (messageInput.value) {
    messageInput.value.style.height = 'auto'
    messageInput.value.style.height = messageInput.value.scrollHeight + 'px'
  }
}

watch(currentMessage, autoResizeTextarea)

// Update context based on current route
const updateContext = () => {
  const path = window.location.pathname
  if (path.includes('/admins')) {
    currentPage.value = 'Admin Management'
  } else if (path.includes('/stores')) {
    currentPage.value = 'Store Management'
  } else if (path.includes('/email')) {
    currentPage.value = 'Email Management'
  } else if (path.includes('/settings')) {
    currentPage.value = 'Settings'
  } else {
    currentPage.value = 'Dashboard'
  }
}

// Lifecycle
onMounted(() => {
  updateContext()
  // Listen for route changes
  window.addEventListener('popstate', updateContext)
})

onUnmounted(() => {
  window.removeEventListener('popstate', updateContext)
})
</script>

<style scoped>
.ai-assistant {
  position: fixed;
  bottom: 20px;
  right: 20px;
  z-index: 1000;
  font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
}

/* Toggle Button */
.ai-toggle-btn {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 12px 16px;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  border: none;
  border-radius: 50px;
  cursor: pointer;
  box-shadow: 0 4px 12px rgba(102, 126, 234, 0.3);
  transition: all 0.3s ease;
  font-size: 14px;
  font-weight: 500;
}

.ai-toggle-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 6px 20px rgba(102, 126, 234, 0.4);
}

.ai-toggle-btn.active {
  background: linear-gradient(135deg, #764ba2 0%, #667eea 100%);
}

.ai-icon {
  width: 20px;
  height: 20px;
}

.ai-label {
  white-space: nowrap;
}

/* AI Panel */
.ai-panel {
  position: absolute;
  bottom: 70px;
  right: 0;
  width: 400px;
  height: 600px;
  background: white;
  border-radius: 16px;
  box-shadow: 0 20px 40px rgba(0, 0, 0, 0.15);
  display: flex;
  flex-direction: column;
  overflow: hidden;
  transition: all 0.3s ease;
  border: 1px solid rgba(0, 0, 0, 0.1);
}

.ai-panel.minimized {
  height: 60px;
}

/* Header */
.ai-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 16px 20px;
  border-bottom: 1px solid #e5e7eb;
  background: #f9fafb;
}

.ai-title {
  display: flex;
  align-items: center;
  gap: 8px;
  font-weight: 600;
  color: #111827;
}

.ai-brain-icon {
  width: 20px;
  height: 20px;
  color: #667eea;
}

.ai-controls {
  display: flex;
  gap: 8px;
}

.control-btn {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 32px;
  height: 32px;
  border: none;
  background: transparent;
  color: #6b7280;
  border-radius: 6px;
  cursor: pointer;
  transition: all 0.2s ease;
}

.control-btn:hover {
  background: #e5e7eb;
  color: #374151;
}

/* Content */
.ai-content {
  flex: 1;
  display: flex;
  flex-direction: column;
  overflow: hidden;
}

/* Quick Actions */
.quick-actions {
  padding: 16px 20px;
  border-bottom: 1px solid #e5e7eb;
}

.quick-actions h4 {
  margin: 0 0 12px 0;
  font-size: 14px;
  font-weight: 600;
  color: #374151;
}

.action-buttons {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 8px;
}

.action-btn {
  display: flex;
  align-items: center;
  gap: 6px;
  padding: 8px 12px;
  background: #f3f4f6;
  border: 1px solid #d1d5db;
  border-radius: 8px;
  font-size: 12px;
  font-weight: 500;
  color: #374151;
  cursor: pointer;
  transition: all 0.2s ease;
}

.action-btn:hover {
  background: #e5e7eb;
  border-color: #9ca3af;
}

.action-btn svg {
  width: 14px;
  height: 14px;
  color: #6b7280;
}

/* Chat Container */
.chat-container {
  flex: 1;
  display: flex;
  flex-direction: column;
  overflow: hidden;
}

.messages {
  flex: 1;
  overflow-y: auto;
  padding: 16px 20px;
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.message {
  display: flex;
  max-width: 80%;
}

.message.user {
  align-self: flex-end;
}

.message.assistant {
  align-self: flex-start;
}

.message-content {
  padding: 12px 16px;
  border-radius: 16px;
  max-width: 100%;
  word-wrap: break-word;
}

.message.user .message-content {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
}

.message.assistant .message-content {
  background: #f3f4f6;
  color: #111827;
}

.message-text {
  font-size: 14px;
  line-height: 1.4;
}

.message-text :deep(p) {
  margin: 0 0 8px 0;
}

.message-text :deep(p:last-child) {
  margin-bottom: 0;
}

.message-text :deep(code) {
  background: rgba(0, 0, 0, 0.1);
  padding: 2px 4px;
  border-radius: 4px;
  font-family: 'Monaco', 'Menlo', monospace;
  font-size: 12px;
}

.message-time {
  font-size: 11px;
  opacity: 0.7;
  margin-top: 4px;
}

/* Typing Indicator */
.typing-indicator {
  display: flex;
  gap: 4px;
  align-items: center;
}

.typing-indicator span {
  width: 6px;
  height: 6px;
  background: #9ca3af;
  border-radius: 50%;
  animation: typing 1.4s infinite;
}

.typing-indicator span:nth-child(2) {
  animation-delay: 0.2s;
}

.typing-indicator span:nth-child(3) {
  animation-delay: 0.4s;
}

@keyframes typing {
  0%, 60%, 100% {
    transform: translateY(0);
    opacity: 0.4;
  }
  30% {
    transform: translateY(-10px);
    opacity: 1;
  }
}

/* Message Input */
.message-input {
  display: flex;
  gap: 8px;
  padding: 16px 20px;
  border-top: 1px solid #e5e7eb;
  background: #f9fafb;
}

.input-field {
  flex: 1;
  padding: 12px 16px;
  border: 1px solid #d1d5db;
  border-radius: 20px;
  font-size: 14px;
  line-height: 1.4;
  resize: none;
  outline: none;
  transition: border-color 0.2s ease;
}

.input-field:focus {
  border-color: #667eea;
}

.send-btn {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 40px;
  height: 40px;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  border: none;
  border-radius: 50%;
  cursor: pointer;
  transition: all 0.2s ease;
}

.send-btn:hover:not(:disabled) {
  transform: scale(1.05);
}

.send-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.send-btn svg {
  width: 18px;
  height: 18px;
}

/* Context Info */
.context-info {
  padding: 12px 20px;
  border-top: 1px solid #e5e7eb;
  background: #f9fafb;
  font-size: 12px;
}

.context-item {
  display: flex;
  justify-content: space-between;
  margin-bottom: 4px;
}

.context-item:last-child {
  margin-bottom: 0;
}

.context-label {
  color: #6b7280;
}

.context-value {
  color: #374151;
  font-weight: 500;
}

/* Responsive */
@media (max-width: 480px) {
  .ai-panel {
    width: calc(100vw - 40px);
    height: calc(100vh - 140px);
    bottom: 100px;
    right: 20px;
  }

  .action-buttons {
    grid-template-columns: 1fr;
  }
}

/* Dark mode support */
@media (prefers-color-scheme: dark) {
  .ai-panel {
    background: #1f2937;
    border-color: #374151;
  }

  .ai-header {
    background: #111827;
    border-color: #374151;
  }

  .ai-title {
    color: #f9fafb;
  }

  .control-btn:hover {
    background: #374151;
  }

  .quick-actions {
    border-color: #374151;
  }

  .action-btn {
    background: #374151;
    border-color: #4b5563;
    color: #f9fafb;
  }

  .action-btn:hover {
    background: #4b5563;
    border-color: #6b7280;
  }

  .messages {
    color: #f9fafb;
  }

  .message.assistant .message-content {
    background: #374151;
    color: #f9fafb;
  }

  .message-input {
    border-color: #374151;
    background: #111827;
  }

  .input-field {
    background: #1f2937;
    border-color: #4b5563;
    color: #f9fafb;
  }

  .input-field:focus {
    border-color: #667eea;
  }

  .context-info {
    border-color: #374151;
    background: #111827;
  }

  .context-label {
    color: #9ca3af;
  }

  .context-value {
    color: #f9fafb;
  }
}
</style>