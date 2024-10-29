package com.arcane.arcana.game.dto;

import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

/**
 * 진행 정보 DTO
 */
@Getter
@Setter
@NoArgsConstructor
@AllArgsConstructor
public class ProgressDto {

    private String userId;
    private String data;
}
