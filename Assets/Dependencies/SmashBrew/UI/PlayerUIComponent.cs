using UnityEngine;
using UnityEngine.EventSystems;

namespace HouraiTeahouse.SmashBrew.UI {

    /// <summary> 
    /// A UI Component that depends on data assigned from a Player object.
    /// </summary>
    public abstract class PlayerUIComponent : UIBehaviour, IDataComponent<Player> {

        Player _player;

        /// <summary> 
        /// The target Player the behaviour represents.
        /// </summary>
        public Player Player {
            get { return _player; }
            set { SetData(value); }
        }

        /// <summary>
        ///     <see cref="IDataComponent{T}.SetData" />
        /// </summary>
        /// <param name="data"> the data to set </param>
        public virtual void SetData(Player data) {
            _player = data;
            PlayerChange();
        }

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        protected override void Awake() {
            base.Awake();
            Mediator.Global.CreateUnityContext(this)
                    .Subscribe<PlayerChanged>(args =>{
                        if (args.Player == Player)
                            PlayerChange();
                    });
        }

        /// <summary> 
        /// Events callback. Called whenever <see cref="Player" />'s state changes.
        /// </summary>
        protected virtual void PlayerChange() {
        }

    }

    /// <summary> 
    /// An abstract UI behaviour class for handling a Scene's data.
    /// </summary>
    public abstract class SceneUIComponent : PlayerUIComponent, IDataComponent<SceneData> {

        [SerializeField]
        [Tooltip("The character whose data is to be displayed")]
        SceneData _scene;

        /// <summary> 
        /// The target Character currently represented by the behaviour.
        /// </summary>
        public SceneData Scene {
            get { return _scene; }
            set { SetData(value); }
        }

        /// <summary>
        ///     <see cref="IDataComponent{T}.SetData" />
        /// </summary>
        public virtual void SetData(SceneData data) {
            _scene = data;
        }

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        protected override void Awake() {
            base.Awake();
            SetData(_scene);
        }

    }

    /// <summary> 
    /// An abstract UI behaviour class for handling a Scene's data 
    /// </summary>
    /// <typeparam name="T"> the type of component the CharacterUIComponent manipulates </typeparam>
    public abstract class SceneUIComponent<T> : PlayerUIComponent<T>, IDataComponent<SceneData> where T : Component {

        [SerializeField]
        [Tooltip("The map whose data is to be displayed")]
        SceneData _scene;

        /// <summary> 
        /// The target scene currently represented by the behaviour 
        /// </summary>
        public SceneData Scene {
            get { return _scene; }
            set { SetData(value); }
        }

        /// <summary>
        ///     <see cref="IDataComponent{T}.SetData" />
        /// </summary>
        public virtual void SetData(SceneData data) {
            _scene = data;
        }

        /// <summary> Unity Callback. Called on object instantiation. </summary>
        protected override void Awake() {
            base.Awake();
            SetData(_scene);
        }

    }

    /// <summary> 
    /// An abstract UI behaviour class for handling a Character's data 
    /// </summary>
    public abstract class CharacterUIComponent : PlayerUIComponent, IDataComponent<CharacterData> {

        [SerializeField]
        [Tooltip("The character whose data is to be displayed")]
        CharacterData _character;

        /// <summary> 
        /// The target Character currently represented by the behaviour 
        /// </summary>
        public CharacterData Character {
            get { return _character; }
            set { SetData(value); }
        }

        /// <summary>
        ///     <see cref="IDataComponent{T}.SetData" />
        /// </summary>
        public virtual void SetData(CharacterData data) {
            _character = data;
        }

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        protected override void Awake() {
            base.Awake();
            SetData(_character);
        }

        /// <summary>
        ///     <see cref="PlayerUIComponent.PlayerChange" />
        /// </summary>
        protected override void PlayerChange() {
            SetData(Player == null ? null : Player.Selection.Character);
        }

    }

    /// <summary> 
    /// An abstract UI behaviour class for handling a Player's current state 
    /// </summary>
    /// <typeparam name="T"> the type of component the PlayerUIComponent manipulates </typeparam>
    public abstract class PlayerUIComponent<T> : PlayerUIComponent where T : Component {

        [SerializeField]
        T _component;

        /// <summary> 
        /// The component the behaviour manipulates 
        /// </summary>
        public T Component {
            get { return _component; }
            protected set { _component = value; }
        }

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        protected override void Awake() {
            base.Awake();
            if (!_component)
                _component = GetComponent<T>();
        }

    }

    /// <summary> 
    /// An abstract UI behaviour class for handling a Character's data 
    /// </summary>
    /// <typeparam name="T"> the type of component the CharacterUIComponent manipulates </typeparam>
    public abstract class CharacterUIComponent<T> : PlayerUIComponent<T>, IDataComponent<CharacterData>
        where T : Component {

        [SerializeField]
        [Tooltip("The character whose data is to be displayed")]
        CharacterData _character;

        /// <summary> 
        /// The target Character currently represented by the behaviour.
        /// </summary>
        public CharacterData Character {
            get { return _character; }
            set { SetData(value); }
        }

        /// <summary>
        ///     <see cref="IDataComponent{T}.SetData" />
        /// </summary>
        public virtual void SetData(CharacterData data) {
            _character = data;
        }

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        protected override void Awake() {
            base.Awake();
            SetData(_character);
        }

        /// <summary>
        ///     <see cref="PlayerUIComponent.PlayerChange" />
        /// </summary>
        protected override void PlayerChange() {
            SetData(Player != null ? Player.Selection.Character : null);
        }

    }

}