import {
  validateString,
  validateWorkspacePath,
  validateSymbolName,
  validateFilePath,
  ValidationError,
} from '../src/index.js';

describe('Input Validation', () => {
  describe('validateString', () => {
    it('should validate string within bounds', () => {
      expect(validateString('test', 'test', 1, 10)).toBe('test');
    });

    it('should throw on string too short', () => {
      expect(() => validateString('', 'test', 1, 10)).toThrow(ValidationError);
    });

    it('should throw on string too long', () => {
      expect(() => validateString('a'.repeat(11), 'test', 1, 10)).toThrow(ValidationError);
    });
  });

  describe('validateWorkspacePath', () => {
    it('should validate valid workspace path', () => {
      expect(validateWorkspacePath('/valid/path')).toBe('/valid/path');
    });

    it('should throw on path traversal', () => {
      expect(() => validateWorkspacePath('../../../etc/passwd')).toThrow(ValidationError);
    });
  });

  describe('validateSymbolName', () => {
    it('should validate valid symbol name', () => {
      expect(validateSymbolName('MyClass')).toBe('MyClass');
    });

    it('should throw on invalid symbol name', () => {
      expect(() => validateSymbolName('')).toThrow(ValidationError);
    });
  });

  describe('validateFilePath', () => {
    it('should validate valid file path', () => {
      expect(validateFilePath('src/index.ts', '/workspace')).toBe('src/index.ts');
    });

    it('should throw on path traversal', () => {
      expect(() => validateFilePath('../../../etc/passwd', '/workspace')).toThrow(ValidationError);
    });
  });
});
