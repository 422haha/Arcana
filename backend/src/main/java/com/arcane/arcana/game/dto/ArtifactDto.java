package com.arcane.arcana.game.dto;

import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

import java.util.List;

/**
 * 아티팩트 DTO
 */
@Getter
@Setter
@NoArgsConstructor
@AllArgsConstructor
public class ArtifactDto {

    private String userId;
    private List<String> artifacts;
}
