package com.arcane.arcana.game.dto;

import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

/**
 * 맵 설정 DTO
 */
@Getter
@Setter
@NoArgsConstructor
@AllArgsConstructor
public class MapSettingDto {

    private String userId;
    private String mapSetting; // S3 JSON 파일 경로
}
