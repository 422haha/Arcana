package com.arcane.arcana.game.service;

import com.arcane.arcana.game.dto.MapSettingDto;
import com.arcane.arcana.game.dto.ItemDto;
import com.arcane.arcana.game.dto.ArtifactDto;
import com.arcane.arcana.game.dto.ProgressDto;

public interface GameDataService {

    void saveMapSetting(String email, MapSettingDto mapSettingDto);

    void saveItem(String email, ItemDto itemDto);

    void saveArtifact(String email, ArtifactDto artifactDto);

    ProgressDto getProgress(String email);

    ItemDto getItem(String email);

    ArtifactDto getArtifact(String email);
}
