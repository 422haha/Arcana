package com.arcane.arcana.game.service;

import com.arcane.arcana.game.dto.ProgressDataDto;
import org.springframework.web.multipart.MultipartFile;

public interface ProgressDataService {

    void saveProgress(Long userId, MultipartFile progress);

    ProgressDataDto getProgress(Long userId);
}
