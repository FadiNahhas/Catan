using System;
using TMPro;
using UnityEngine;

namespace Helpers
{
    [Serializable]
    [CreateAssetMenu (fileName = "PasswordInputValidator", menuName = "Helpers/PasswordInputValidator")]
    public class PasswordInputValidator : TMP_InputValidator
    {
        private const string AllowedCharsPattern = @"^[a-zA-Z0-9!@#$%^&*()]*$";

        public override char Validate(ref string text, ref int pos, char ch)
        {
            if (ch == '\n' || ch == '\r')
            {
                return '\0';
            }

            if (ch == '\b')
            {
                if (pos == 0)
                {
                    return '\0';
                }

                pos--;
                text = text.Remove(pos, 1);
                return '\0';
            }

            if (text.Length >= 16)
            {
                return '\0';
            }

            if (System.Text.RegularExpressions.Regex.IsMatch(ch.ToString(), AllowedCharsPattern))
            {
                text = text.Insert(pos, ch.ToString());
                pos++;
            }

            return '\0';
        }
    }
}