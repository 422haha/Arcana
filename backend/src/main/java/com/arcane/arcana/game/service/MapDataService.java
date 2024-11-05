package com.arcane.arcana.game.service;

import com.arcane.arcana.game.dto.MapDataDto;
import org.springframework.web.multipart.MultipartFile;

public interface MapDataService {

    /*
    void saveMapSetting(Long userId, MultipartFile mapSetting);
    */

    void saveMapSetting(Long userId, String mapSetting);

    MapDataDto getMapSetting(Long userId);
}
