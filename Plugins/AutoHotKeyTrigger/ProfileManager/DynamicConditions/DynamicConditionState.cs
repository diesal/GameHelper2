﻿// <copyright file="DynamicConditionState.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutoHotKeyTrigger.ProfileManager.DynamicConditions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GameHelper.RemoteEnums;
    using GameHelper.RemoteObjects.Components;
    using GameHelper.RemoteObjects.States;
    using ClickableTransparentOverlay.Win32;
    using AutoHotKeyTrigger.ProfileManager.DynamicConditions.Interface;
    using System.Linq.Dynamic.Core.CustomTypeProviders;
    using GameOffsets.Objects.Components;

    /// <summary>
    ///     The structure that can be queried using DynamicCondition
    /// </summary>
    [DynamicLinqType]
    public class DynamicConditionState : IDynamicConditionState
    {
        private readonly Lazy<NearbyMonsterInfo> nearbyMonsterInfo;

        /// <summary>
        ///     Creates a new instance
        /// </summary>
        /// <param name="state">State to build the structure from</param>
        public DynamicConditionState(InGameState state)
        {
            if (state != null)
            {
                var player = state.CurrentAreaInstance.Player;
                if (player.TryGetComponent<Buffs>(out var playerBuffs))
                {
                    this.PlayerAilments = JsonDataHelper.StatusEffectGroups
                                                  .Where(x => x.Value.Any(playerBuffs.StatusEffects.ContainsKey))
                                                  .Select(x => x.Key).ToHashSet();
                    this.PlayerBuffs = new BuffDictionary(playerBuffs.StatusEffects);
                }

                if (player.TryGetComponent<Actor>(out var actorComponent))
                {
                    this.PlayerAnimation = actorComponent.Animation;
                    this.PlayerSkillIsUseable = actorComponent.IsSkillUsable;
                    this.DeployedObjectsCount = actorComponent.DeployedEntities;
                    this.ActiveSkills = actorComponent.ActiveSkills;
                }

                if (player.TryGetComponent<Life>(out var lifeComponent))
                {
                    this.PlayerVitals = new VitalsInfo(lifeComponent);
                }

                if (player.TryGetComponent<Stats>(out var statsComp))
                {
                    this.PlayerFirstWeaponSetActive = statsComp.CurrentWeaponIndex == 0;
                    this.PlayerSecondWeaponSetActive = statsComp.CurrentWeaponIndex == 1;
                    this.PlayerIsShapeShifted = statsComp.IsInShapeshiftedForm;
                }
                else
                {
                    this.PlayerFirstWeaponSetActive = false;
                    this.PlayerSecondWeaponSetActive = false;
                    this.PlayerIsShapeShifted = false;
                }

                this.Flasks = new FlasksInfo(state);
                this.nearbyMonsterInfo = new Lazy<NearbyMonsterInfo>(() => new NearbyMonsterInfo(state));
            }
        }

        /// <summary>
        ///     The buff list
        /// </summary>
        public IBuffDictionary PlayerBuffs { get; }

        /// <summary>
        ///     The current animation
        /// </summary>
        public int PlayerAnimation { get; }

        /// <summary>
        ///     The player skill useability status.
        /// </summary>
        public HashSet<string> PlayerSkillIsUseable { get; } = new();

        /// <summary>
        ///   The player skill details are in this structure.
        /// </summary>
        public Dictionary<string, ActiveSkillDetails> ActiveSkills { get; } = new();

        /// <summary>
        ///     The objects deployed by the player with Object type as key and Object Counter as value.
        /// </summary>
        public int[] DeployedObjectsCount { get; } = new int[256];

        /// <summary>
        ///     The ailment list
        /// </summary>
        public IReadOnlyCollection<string> PlayerAilments { get; } = new List<string>();

        /// <summary>
        ///     The vitals information
        /// </summary>
        public IVitalsInfo PlayerVitals { get; }

        /// <summary>
        ///     The flask information
        /// </summary>
        public IFlasksInfo Flasks { get; }

        /// <summary>
        ///     Calculates the number of nearby monsters given a rarity selector in the outer circle.
        /// </summary>
        /// <param name="rarity">The rarity selector for monster search</param>
        /// <returns></returns>
        public int MonsterCount(MonsterRarity rarity) =>
            this.nearbyMonsterInfo.Value.GetMonsterCount(rarity, MonsterNearbyZones.OuterCircle);

        /// <summary>
        ///     Calculates the number of nearby monsters given a rarity selector
        /// </summary>
        /// <param name="rarity">The rarity selector for monster search</param>
        /// <param name="zone">circle in which the monster exists</param>
        /// <returns></returns>
        public int MonsterCount(MonsterRarity rarity, MonsterNearbyZones zone) =>
            this.nearbyMonsterInfo.Value.GetMonsterCount(rarity, zone);

        /// <summary>
        ///     Number of friendly nearby monsters in the inner circle.
        /// </summary>
        public int InnerCircleFriendlyMonsterCount =>
            this.nearbyMonsterInfo.Value.FriendlyCount[0];

        /// <summary>
        ///     Number of friendly nearby monsters in the outer circle.
        /// </summary>
        public int OuterCircleFriendlyMonsterCount =>
            this.nearbyMonsterInfo.Value.FriendlyCount[1];

        /// <summary>
        ///     For backword compatibility reasons: <inheritdoc cref="OuterCircleFriendlyMonsterCount"/>
        /// </summary>
        public int FriendlyMonsterCount => this.OuterCircleFriendlyMonsterCount;

        /// <summary>
        ///     Gets the value indicating if first weapon set is active or not.
        /// </summary>
        public bool PlayerFirstWeaponSetActive { get; }

        /// <summary>
        ///     Gets the value indicating if second weapon set is active or not.
        /// </summary>
        public bool PlayerSecondWeaponSetActive { get; }

        /// <summary>
        ///     Gets the value indicating if player is shapeshifted or not.
        /// </summary>
        public bool PlayerIsShapeShifted { get; }

        /// <summary>
        ///     Capture the key press event
        /// </summary>
        public bool IsKeyPressedForAction(VK vk) => Utils.IsKeyPressed(vk);
    }
}
