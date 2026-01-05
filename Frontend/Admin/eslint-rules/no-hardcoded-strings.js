/**
 * ESLint rule to detect hardcoded strings in Vue templates
 * Flags user-facing text that should be internationalized with $t() or v-t
 */

export default {
  meta: {
    type: 'problem',
    docs: {
      description: 'Disallow hardcoded strings in Vue templates',
      category: 'i18n',
      recommended: true,
    },
    fixable: null,
    schema: [
      {
        type: 'object',
        properties: {
          ignoreText: {
            type: 'array',
            items: { type: 'string' },
          },
          ignoreAttributes: {
            type: 'array',
            items: { type: 'string' },
          },
          ignoreDirectives: {
            type: 'array',
            items: { type: 'string' },
          },
          minLength: {
            type: 'number',
            minimum: 1,
          },
        },
        additionalProperties: false,
      },
    ],
    messages: {
      hardcodedString: 'Hardcoded string "{{text}}" should be internationalized with $t() or v-t directive',
    },
  },

  create(context) {
    const options = context.options[0] || {};
    const ignoreText = new Set(options.ignoreText || ['&nbsp;', ' ', '\n', '\t', '\r']);
    const ignoreAttributes = new Set(options.ignoreAttributes || ['placeholder', 'alt', 'title', 'aria-label']);
    const ignoreDirectives = new Set(options.ignoreDirectives || ['t']);
    const minLength = options.minLength || 3;

    return {
      Program(node) {
        // Only process Vue files
        if (!context.filename.endsWith('.vue')) {
          return;
        }

        const sourceCode = context.sourceCode.getText();
        
        // Extract template content (rough approximation)
        const templateMatch = sourceCode.match(/<template>([\s\S]*?)<\/template>/);
        if (!templateMatch) {
          return;
        }

        const templateContent = templateMatch[1];
        
        // Find text content between tags (simple regex approach)
        const textMatches = templateContent.match(/>([^<>{}]+)</g);
        
        if (textMatches) {
          textMatches.forEach(match => {
            const text = match.slice(1, -1).trim(); // Remove > and <
            
            // Skip empty or ignored text
            if (!text || ignoreText.has(text)) {
              return;
            }

            // Skip text that looks like variables, expressions, or punctuation
            if (
              text.startsWith('{{') ||
              text.endsWith('}}') ||
              /^\W+$/.test(text) || // Only punctuation/symbols
              /^\d+$/.test(text) || // Only numbers
              /^[a-zA-Z]{1,2}$/.test(text) || // Very short words (likely abbreviations)
              /^https?:\/\//.test(text) || // URLs
              /^[\w.-]+@[\w.-]+\.\w+$/.test(text) // Email addresses
            ) {
              return;
            }

            // Flag text that appears to be user-facing content
            if (text.length >= minLength && /[a-zA-Z]/.test(text)) {
              // Find the position in the source
              const index = sourceCode.indexOf(match);
              const loc = context.sourceCode.getLocFromIndex(index + 1); // +1 to skip the >
              
              context.report({
                loc,
                messageId: 'hardcodedString',
                data: { text: text.length > 20 ? text.substring(0, 20) + '...' : text },
              });
            }
          });
        }
      },
    };
  },
};