﻿namespace Hourai.Events {

    public class GlobalEventManager : Mediator {

        private static GlobalEventManager _instance;

        public static GlobalEventManager Instance => _instance ?? (_instance = new GlobalEventManager());

        private GlobalEventManager() {
        }

    }


}