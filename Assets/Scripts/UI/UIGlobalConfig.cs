using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace independent
{

    static class ConstDesignSize
    {
        public const float width = 1920;

        public const float height = 1080;

        public const int PopupConstIndex = 100;

    }

    public enum AdaptionType
    {
        None = 0,
        Top = 1,
        Bottom = 2,
        Left = 3,
        Right = 4,
        Center = 5
    }

    public enum UIMgrType
    {
        center = 0,
    }

    public static class UIConstConfig
    {
        public static List<UIConfig> configList = new List<UIConfig>(){
            new UIConfig("Content",new Vector2(0.5f,0.5f),new Vector2(0,0),new Vector2(1,1),UIMgrType.center,AdaptionType.Center),
        };
    }

    public static class ConstBangsWidth
    {
        /**
     * 刘海适配的宽度
     */
        public const float width = 550f;
    }


    public class UIConfig
    {
        public string name;
        public Vector2 pivot;

        public Vector2 pos;

        public Vector2 size;

        public UIMgrType compT;

        public AdaptionType adaptionType;

        public UIConfig(string name, Vector2 pivot, Vector2 pos, Vector2 size, UIMgrType comp, AdaptionType type)
        {
            this.name = name;
            this.pivot = pivot;
            this.pos = pos;
            this.size = size;
            this.compT = comp;
            this.adaptionType = type;
        }
    }



    // ******************************************************************************************************//
    // * popuper组件配置
    // ******************************************************************************************************//	
    static class PopuperConfig
    {
        public static class maskPrefab
        {
            public static string maskPath = "db://assets/resources/common/res-common/prefab/ui/*";
            public static string maskURL = "Assets/GameAssets/Prefabs/UI/PopupMask.prefab";
        };

        public static class stencil
        {
            public static string popupMask = "popupMask";
            public static string popupNode = "popupNode";
            public static string popupAdapt = "popupAdapt";
            public static string nodeBg = "background";
            public static string nodeContent = "content";
            public static string nodeCloseBtn = "closeButton";
        }

        public static class stencilSize
        {
            public static Vector2 popupMask = new Vector2(800, 600);
            public static Vector2 buttonSize = new Vector2(200, 150);
            public static Vector2 designSize = new Vector2(ConstDesignSize.width, ConstDesignSize.height);
        }

        public const string ConstPopupLayer = "popupLayer";

        public const string ConstAdaptBg = "adapt-bg";

        public const string ConstAdaptContent = "adapt-content";
    }

    public enum SceneType
    {
        SCENE_LOGIN = 0,                            // 登陆场景
        SCENE_LOBBY = 1,                            // 大厅场景
        SCENE_LOADING = 2,                      // 加载页场景

        SCENE_GAME = 3,                      // game场景
    }

    public enum BangsSet
    {
        FIXED = 0,
        CUSTOM = 1,
    }

}


// public const PopuperConfig = {
// 	// 对话框的mask所需要的预制路径配置
// 	maskPrefab: {
//         maskPath: 'db://assets/resources/common/res-common/prefab/ui/**\/*',
//         maskURL: 'db://assets/resources/common/res-common/prefab/ui/PopupMask.prefab'
//     },
//     stencil: {
//         popupMask: 'popupMask',
//         popupNode: 'popupNode',
//         popupAdapt: 'popupAdapt',
//         nodeBg: 'background',
//         nodeContent: 'content',
//         nodeCloseBtn: 'closeButton'
//     },
//     stencilSize: {
//         popupSize: cc.size(800, 600),
//         buttonSize: cc.size(200, 100),
//         designSize: ConstDesignSize
//     }
// }
