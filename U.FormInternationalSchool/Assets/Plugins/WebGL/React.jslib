mergeInto(LibraryManager.library, 
{
  OnGameCreated: function (gameId, gameType) {
    window.dispatchReactUnityEvent("OnGameCreated", gameId, gameType);
  },
  OnShowPreview: function (gameType, json) {
      window.dispatchReactUnityEvent("OnShowPreview", gameType, json);
    },
});

