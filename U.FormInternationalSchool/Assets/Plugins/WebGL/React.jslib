mergeInto(LibraryManager.library, 
{
  OnGameCreated: function (gameId, gameType) {
    window.dispatchReactUnityEvent("OnGameCreated", gameId, UTF8ToString(gameType));
  },
  OnShowPreview: function (gameType, json) {
      window.dispatchReactUnityEvent("OnShowPreview", UTF8ToString(gameType), UTF8ToString(json));
    },
  AddGameToPath: function (gameId, gameType) {
      window.dispatchReactUnityEvent("AddGameToPath", gameId, UTF8ToString(gameType));
    }
});

