mergeInto(LibraryManager.library, 
{
  OnGameCreated: function (gameId, gameType) {
    window.onGameCreated("OnGameCreated", gameId, UTF8ToString(gameType));
  },
  OnShowPreview: function (gameType, json) {
      window.onShowPreview("OnShowPreview", UTF8ToString(gameType), UTF8ToString(json));
    },
  AddGameToPath: function (gameId, gameType) {
      window.addGameToPath("AddGameToPath", gameId, UTF8ToString(gameType));
    }
});

