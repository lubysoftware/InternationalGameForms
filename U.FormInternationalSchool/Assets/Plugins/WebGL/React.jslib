mergeInto(LibraryManager.library, 
{
  OnGameCreated: function (gameId, gameType) {
    window.dispatchReactUnityEvent("OnGameCreated", gameId, gameType);
  },
});