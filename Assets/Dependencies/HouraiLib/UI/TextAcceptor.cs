using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HouraiTeahouse {

    /// <summary>
    /// Any component that can accept a 
    /// </summary>
    public interface ITextAcceptor {

        /// <summary>
        /// The selection priority used by <see cref="ITextAcceptorExtensions.SetUIText(GameObject, string)"/>.
        /// Higher priority ITextAcceptors will be accepted over lower priority.
        /// </summary>
        /// <returns> the prioirty of the acceptor </returns>
        int Priority { get; }

        /// <summary>
        /// Sets the visible text of the object.
        /// </summary>
        /// <param name="text"> the text string to assign to the object.</param>
        void SetText(string text);

    }

    /// <summary>
    /// A wrapper ITextAcceptor around a UnityEngine.UI.Text component.
    /// </summary>
    class UITextAcceptor : ITextAcceptor {

        readonly Text _text;

        /// <summary>
        /// The selection priority used by <see cref="ITextAcceptorExtensions.SetUIText(GameObject, string)"/>.
        /// Higher priority ITextAcceptors will be accepted over lower priority.
        /// </summary>
        /// <returns> the prioirty of the acceptor </returns>
        public int Priority { get; private set; }

        internal UITextAcceptor(Text text) {
            _text = Argument.NotNull(text);
        }

        public void SetText(string text) {
            if (_text != null)
                _text.text = text;
        }

    }

    /// <summary>
    /// A wrapper ITextAcceptor around a TMPro.TMP_Text component.
    /// </summary>
    class TMPTextAcceptor : ITextAcceptor {

        readonly TMP_Text _text;

        public int Priority { get; private set; }

        internal TMPTextAcceptor(TMP_Text text) {
            _text = Argument.NotNull(text);
        }

        public void SetText(string text) {
            if (_text != null)
                _text.text = text;
        }

    }

    public static class ITextAcceptorExtensions {

        /// <summary>
        /// Sets the UI text of an GameObject.
        /// Searches for Components implementing <see cref="ITextAcceptor"/> in the child objects of
        /// <paramref cref="gameObject"/> and injects <paramref cref="text"/> as it's Text.
        /// </summary>
        /// <remarks>
        /// If no implementation is found, a UnityEngine.UI.Text or TMPro.TMP_Text is has it's text directly assigned.
        /// Only one ITextAcceptor will be used: the one with the highest Priority.
        /// </remarks>
        /// <param name="gameObject"> the root GameObject to search for a implementor. </param>
        /// <param name="text"> the text string to assign</param>
        /// <returns> the utilized ITextAcceptor that had it's text set. </returns>
        public static ITextAcceptor SetUIText(this GameObject gameObject, string text) {
            var acceptors = Argument.NotNull(gameObject).GetComponentsInChildren<ITextAcceptor>();
            if (acceptors.Any()) {
                var acceptor = acceptors.OrderByDescending(s => s.Priority).First();
                acceptor.SetText(text);
                return acceptor;
            }
            var uiText = gameObject.GetComponentInChildren<Text>();
            if (uiText != null) {
                var acceptor = new UITextAcceptor(uiText);
                acceptor.SetText(text);
                return acceptor;
            }
            var tmpText = gameObject.GetComponentInChildren<TMP_Text>();
            if (tmpText != null) {
                var acceptor = new TMPTextAcceptor(tmpText);
                acceptor.SetText(text);
                return acceptor;
            }
            return null;
        }

    }


}